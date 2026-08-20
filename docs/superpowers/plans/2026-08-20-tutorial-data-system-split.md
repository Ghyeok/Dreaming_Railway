# TutorialData / TutorialSystem 분리 구현 계획

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** `TutorialManager`의 public 필드 19개를 순수 C# `TutorialData`(상태 8 + 파생 프로퍼티 11)와 `TutorialSystem`(진행 조정)으로 분리하고, 매 프레임 플래그를 재계산하던 `SetTutorialTrigger()`를 제거한다.

**Architecture:** 프로젝트의 기존 패턴을 그대로 따른다 — 데이터는 `GameDataManager`가 소유하는 순수 C# 클래스, 이벤트는 데이터 클래스에 두고, 로직은 상태 없는 `*System`이 담당한다. 파생 플래그 8개는 세터 없는 계산 프로퍼티가 되어 상태에서 사라진다. `TutorialManager`는 삭제되고 `TutorialSystem`이 `ManagerInitializer` 목록에서 그 자리를 대체한다.

**Tech Stack:** Unity 6.3 (6000.3.8f1), C#, 단일 `Assembly-CSharp` (asmdef 없음)

**Spec:** `docs/superpowers/specs/2026-08-20-tutorial-data-system-split-design.md`

## ⚠️ TDD 불가 — 검증 방식이 다르다

이 프로젝트는 **asmdef가 0개**라 모든 코드가 predefined `Assembly-CSharp`에 컴파일된다. Unity의 assembly definition은 **predefined 어셈블리를 참조할 수 없으므로**, 테스트 asmdef를 만들어도 `TutorialData`가 보이지 않는다. 단위 테스트를 하려면 `TutorialData`를 별도 asmdef로 분리해야 하고 이는 이 계획의 범위 밖이다.

따라서 각 Task는 **"실패하는 테스트 → 구현 → 통과"** 대신 다음 순서를 따른다:

1. 구현
2. **컴파일 검증** (아래 명령)
3. **정적 검사** (grep 기반 — 잔존 참조 0건 확인)
4. 커밋

```bash
dotnet msbuild Assembly-CSharp.csproj -t:Build -p:BuildProjectReferences=false -v:m -nologo 2>&1 | grep -c "error CS"
# 기대값: 0
```

> 이 명령은 Unity가 이 세션에서 한 번 이상 컴파일했어야 동작한다. `Temp/bin/Debug/`가 비어 있으면 먼저 `cp Library/ScriptAssemblies/*.dll Temp/bin/Debug/`를 실행한다.

**행동 검증은 Task 7의 수동 플레이로만 가능하다.** Task 6까지 끝나기 전에는 "완료"라고 보고하지 않는다.

## Global Constraints

- Unity 6.3 / 단일 `Assembly-CSharp` — 새 asmdef를 만들지 않는다.
- 런타임 스크립트에서 `using UnityEditor.*` 금지 (에디터에선 컴파일되지만 플레이어 빌드가 깨진다).
- **데이터 클래스는 `GameManager` / `UIManager` / 어떤 매니저도 참조하지 않는다.** 매니저가 필요한 값은 파라미터로 받는다.
- **데이터 인스턴스는 `GameDataManager`만 생성한다.** 다른 곳에서 `new TutorialData()` 금지.
- **이벤트는 데이터 클래스에 둔다.** 시스템에 두지 않는다.
- `SingletonManagers<T>.Instance`는 `OnDestroy` 이후 **null을 반환**한다. `OnDisable`/`OnDestroy`에서 `.Instance`를 호출하지 말고 `Awake`/`Init`에서 필드에 캐싱한 참조로 해제한다.
- 파일 인코딩은 기존과 동일하게 UTF-8 BOM을 유지한다.

## ⚠️ Task 4~6 사이에는 게임을 실행하지 않는다

Task 4에서 소비자가 `TutorialSystem.Instance`에 접근하는 순간 시스템이 자동 생성되는데, `TutorialManager`는 Task 6에서야 삭제된다. 그 사이에는 **둘 다 `Player.OnNearExit`을 구독**해 핸들러가 이중 발화한다. Task 6을 마치기 전 플레이 테스트는 무의미하다.

## 파일 구조

| 파일 | 책임 |
|---|---|
| `Assets/Scripts/Content/Tutorial/Data/TutorialConfigData.cs` (수정) | 트리거 인덱스 상수 — 매직넘버 8/9 승격 |
| `Assets/Scripts/Content/Tutorial/Data/TutorialData.cs` (신규) | 튜토리얼 상태 + 파생 프로퍼티 + 이벤트 |
| `Assets/Scripts/Content/Tutorial/System/TutorialSystem.cs` (신규) | Player 이벤트 구독, 진행 조정, 팝업 참조 |
| `Assets/Scripts/Content/Tutorial/TutorialManager.cs` (삭제) | — |
| `Assets/Scripts/Core/Managers/GameDataManager.cs` (수정) | `Tutorial` 소유 + `ResetForNewRun()` 합류 |
| `Assets/Scripts/Core/Managers/GameManager.cs` (수정) | `ResetTutorial()` 별도 호출 제거 |
| `Assets/Scripts/Utils/ManagerInitializer.cs` (수정) | `TutorialManager` → `TutorialSystem` |
| `UI_TutorialPopup.cs` / `DreamManager.cs` / `UI_GameOverPopup.cs` / `UI_GameClearPopup.cs` / `UI_SubwayScene.cs` (수정) | 호출부 전환 |

---

### Task 1: TutorialConfigData 상수 추가 + TutorialData 생성

**Files:**
- Modify: `Assets/Scripts/Content/Tutorial/Data/TutorialConfigData.cs`
- Create: `Assets/Scripts/Content/Tutorial/Data/TutorialData.cs`

**Interfaces:**
- Consumes: `TutorialConfigData`의 기존 상수 (`SLAP_IDX`, `STANDING_IDX`, `SKIP_IDX`, `ENTER_DREAM_IDX`, `SUBWAY_END_IDX`, `GAME_CLEAR_IDX`, `MOVE_IDX`, `EXIT_IDX`, `DARK_GAMEOVER_IDX`, `PASS_GAMEOVER_IDX`)
- Produces: `TutorialPhase`, `GameOverKind`, `TutorialData` — 모든 public 멤버가 이후 Task에서 쓰인다.

- [ ] **Step 1: `TutorialConfigData`에 상수 2개 추가**

기존 `GAME_CLEAR_IDX = 28;` 줄 바로 아래에 추가한다.

```csharp
    // UI 힌트 노출 시점 — 기존 UI_TutorialPopup.Update()의 매직넘버 8 / 9
    public const int TIREDNESS_HINT_IDX = 8;
    public const int TRANSFER_HINT_IDX = 9;
```

- [ ] **Step 2: `TutorialData.cs` 생성**

```csharp
using System;
using UnityEngine;

public enum TutorialPhase { Subway, Dream, GameOver }
public enum GameOverKind { Dark, Passed }

/// <summary>
/// 튜토리얼 진행 상태. 씬과 무관하게 GameDataManager가 소유한다.
/// 파생 플래그(IsSlapStep 등)는 상태가 아니라 인덱스에서 계산되는 값이라 세터가 없다.
/// 뷰 참조(UI_TutorialPopup)는 여기 두지 않는다 — TutorialSystem의 몫.
/// </summary>
[System.Serializable]
public class TutorialData
{
    // ── 진짜 상태
    [field: SerializeField] public TutorialPhase Phase { get; private set; } = TutorialPhase.Subway;
    [field: SerializeField] public int SubwayIdx { get; private set; } = 0;
    [field: SerializeField] public int DreamIdx { get; private set; } = 0;
    [field: SerializeField] public int GameOverIdx { get; private set; } = 0;
    [field: SerializeField] public bool StartFlowTime { get; private set; } = false;
    [field: SerializeField] public bool IsGameOverActive { get; private set; } = false;
    [field: SerializeField] public GameOverKind OverKind { get; private set; } = GameOverKind.Dark;

    /// <summary>
    /// 현재 단계를 이미 소비했는가를 기록한다. 인덱스가 바뀌면 자동으로 무효화된다.
    /// 예전에 외부에서 `isMoveTutorial = false`로 끄던 래치를 대체한다.
    /// </summary>
    private int _consumedIdx = -1;

    public event Action OnStepChanged;
    public event Action<bool> OnFlowTimeChanged;

    // ── 파생 (getter-only)

    /// <summary>현재 Phase가 가리키는 인덱스</summary>
    private int CurrentIdx => Phase switch
    {
        TutorialPhase.Subway => SubwayIdx,
        TutorialPhase.Dream => DreamIdx,
        _ => GameOverIdx,
    };

    private bool IsConsumed => _consumedIdx == CurrentIdx;

    public bool IsSlapStep => Phase == TutorialPhase.Subway && SubwayIdx == TutorialConfigData.SLAP_IDX && !IsConsumed;
    public bool IsStandingStep => Phase == TutorialPhase.Subway && SubwayIdx == TutorialConfigData.STANDING_IDX && !IsConsumed;
    public bool IsSkipStep => Phase == TutorialPhase.Subway && SubwayIdx == TutorialConfigData.SKIP_IDX && !IsConsumed;
    public bool IsEnterDreamStep => Phase == TutorialPhase.Subway && SubwayIdx == TutorialConfigData.ENTER_DREAM_IDX && !IsConsumed;
    public bool IsSubwayEndStep => Phase == TutorialPhase.Subway && SubwayIdx == TutorialConfigData.SUBWAY_END_IDX && !IsConsumed;
    public bool IsGameClearStep => Phase == TutorialPhase.Subway && SubwayIdx == TutorialConfigData.GAME_CLEAR_IDX && !IsConsumed;
    public bool IsMoveStep => Phase == TutorialPhase.Dream && DreamIdx == TutorialConfigData.MOVE_IDX && !IsConsumed;
    public bool IsExitStep => Phase == TutorialPhase.Dream && DreamIdx == TutorialConfigData.EXIT_IDX && !IsConsumed;

    public bool IsTirednessHintStep => Phase == TutorialPhase.Subway && SubwayIdx == TutorialConfigData.TIREDNESS_HINT_IDX;
    public bool IsTransferHintStep => Phase == TutorialPhase.Subway && SubwayIdx == TutorialConfigData.TRANSFER_HINT_IDX;

    /// <summary>
    /// 팝업을 숨기고 플레이어의 조작을 기다려야 하는 단계인가.
    /// 예전 UI_TutorialPopup.Update()의 11개 OR 체인을 대체한다.
    /// </summary>
    public bool IsWaitingForPlayerAction =>
        IsSlapStep || IsStandingStep || IsSkipStep || IsEnterDreamStep ||
        IsSubwayEndStep || IsGameClearStep || IsMoveStep || IsExitStep ||
        (Phase == TutorialPhase.GameOver && IsGameOverActive);

    // ── 변경

    /// <summary>현재 Phase의 인덱스를 1 진행시킨다.</summary>
    public void AdvanceIdx()
    {
        switch (Phase)
        {
            case TutorialPhase.Subway: SubwayIdx++; break;
            case TutorialPhase.Dream: DreamIdx++; break;
            default: GameOverIdx++; break;
        }

        _consumedIdx = -1;

        // 예전 SetTutorialTrigger()가 ENTER_DREAM_IDX에서 startFlowTime을 켜던 자리
        if (Phase == TutorialPhase.Subway && SubwayIdx == TutorialConfigData.ENTER_DREAM_IDX)
            SetFlowTime(true);

        OnStepChanged?.Invoke();
    }

    public void EnterDream()
    {
        Phase = TutorialPhase.Dream;
        _consumedIdx = -1;
        OnStepChanged?.Invoke();
    }

    public void ReturnToSubway()
    {
        Phase = TutorialPhase.Subway;
        _consumedIdx = -1;
        OnStepChanged?.Invoke();
    }

    public void EnterGameOver(GameOverKind kind)
    {
        Phase = TutorialPhase.GameOver;
        IsGameOverActive = true;
        OverKind = kind;
        GameOverIdx = kind == GameOverKind.Dark
            ? TutorialConfigData.DARK_GAMEOVER_IDX
            : TutorialConfigData.PASS_GAMEOVER_IDX;
        _consumedIdx = -1;
        OnStepChanged?.Invoke();
    }

    public void SetFlowTime(bool value)
    {
        if (StartFlowTime == value) return;

        StartFlowTime = value;
        OnFlowTimeChanged?.Invoke(value);
    }

    /// <summary>현재 단계를 소비 처리해 같은 인덱스에서 재발화하지 않게 한다.</summary>
    public void ConsumeCurrentStep() => _consumedIdx = CurrentIdx;

    /// <summary>새 런 시작 시 초기화. GameDataManager.ResetForNewRun()이 호출한다.</summary>
    public void Reset()
    {
        Phase = TutorialPhase.Subway;
        SubwayIdx = 0;
        DreamIdx = 0;
        GameOverIdx = 0;
        IsGameOverActive = false;
        OverKind = GameOverKind.Dark;
        _consumedIdx = -1;

        SetFlowTime(false);
        OnStepChanged?.Invoke();
    }
}
```

- [ ] **Step 3: 컴파일 검증**

```bash
dotnet msbuild Assembly-CSharp.csproj -t:Build -p:BuildProjectReferences=false -v:m -nologo 2>&1 | grep -c "error CS"
```
Expected: `0`

- [ ] **Step 4: 커밋**

```bash
git add Assets/Scripts/Content/Tutorial/Data/
git commit -m "feat: TutorialData 추가 — 파생 플래그를 계산 프로퍼티로 정의"
```

---

### Task 2: GameDataManager가 TutorialData를 소유

**Files:**
- Modify: `Assets/Scripts/Core/Managers/GameDataManager.cs`

**Interfaces:**
- Consumes: `TutorialData` (Task 1)
- Produces: `GameDataManager.Instance.Tutorial` — Task 3~6이 사용한다.

> `GameManager.StartDay()`의 `TutorialManager.Instance.ResetTutorial()` 호출은 **이 Task에서 건드리지 않는다.** Task 6에서 `TutorialManager`를 삭제할 때 함께 제거한다. 그 전까지는 두 리셋이 공존하지만 서로 다른 객체라 충돌하지 않는다.

- [ ] **Step 1: 필드와 프로퍼티 추가**

`private DreamData _dream = new();` 아래에 추가:

```csharp
    [SerializeField] private TutorialData _tutorial = new();
```

`public DreamData Dream => _dream;` 아래에 추가:

```csharp
    public TutorialData Tutorial => _tutorial;
```

- [ ] **Step 2: `ResetForNewRun()`에 합류**

```csharp
    public void ResetForNewRun()
    {
        _tiredness.Reset();
        _timer.Reset();
        _dream.Reset();
        _subway.Reset();
        _tutorial.Reset();
    }
```

- [ ] **Step 3: 컴파일 검증**

```bash
dotnet msbuild Assembly-CSharp.csproj -t:Build -p:BuildProjectReferences=false -v:m -nologo 2>&1 | grep -c "error CS"
```
Expected: `0`

- [ ] **Step 4: 커밋**

```bash
git add Assets/Scripts/Core/Managers/GameDataManager.cs
git commit -m "refactor: GameDataManager가 TutorialData를 소유"
```

---

### Task 3: TutorialSystem 생성

**Files:**
- Create: `Assets/Scripts/Content/Tutorial/System/TutorialSystem.cs`

**Interfaces:**
- Consumes: `GameDataManager.Instance.Tutorial` (Task 2), `Player.OnNearExit` / `Player.OnDreamExit` (static events), `UI_TutorialPopup.AdvanceDialog()`
- Produces:
  - `TutorialSystem.Instance.TutorialPopup` — `UI_TutorialPopup` 타입, get/set
  - `TutorialSystem.Instance.EnterDreamPhase()` → `void`
  - `TutorialSystem.Instance.EnterGameOverPhase(GameOverKind kind)` → `void`
  - `TutorialSystem.Instance.Init()` → `void`

> `ManagerInitializer` 수정은 **Task 6**에서 한다. 이 Task 시점엔 아무도 `TutorialSystem.Instance`를 부르지 않으므로 인스턴스가 생성되지 않는다.

- [ ] **Step 1: `TutorialSystem.cs` 생성**

```csharp
using UnityEngine;

/// <summary>
/// 튜토리얼 진행 조정. 상태는 갖지 않고 TutorialData를 구동한다.
/// Player의 static 이벤트를 구독해야 하므로 꿈 씬 로드 전에 존재해야 한다
/// (그래서 ManagerInitializer 목록에 들어간다 — DreamManager와 같은 이유).
/// </summary>
public class TutorialSystem : SingletonManagers<TutorialSystem>, IManager
{
    private TutorialData _data;

    /// <summary>뷰 참조 — 데이터가 아니므로 TutorialData가 아니라 여기 둔다.</summary>
    public UI_TutorialPopup TutorialPopup { get; set; }

    public void Init()
    {
        _data = GameDataManager.Instance.Tutorial;
    }

    private void OnEnable()
    {
        Player.OnNearExit += HandleNearExit;
        Player.OnDreamExit += HandleDreamExit;
    }

    private void OnDisable()
    {
        Player.OnNearExit -= HandleNearExit;
        Player.OnDreamExit -= HandleDreamExit;
    }

    /// <summary>꿈 씬 진입 — 예전 DreamManager가 필드 4개를 직접 대입하던 자리.</summary>
    public void EnterDreamPhase()
    {
        _data ??= GameDataManager.Instance.Tutorial;

        _data.EnterDream();
        _data.SetFlowTime(false);

        if (_data.DreamIdx < TutorialConfigData.EXIT_IDX)
            TutorialPopup = UIManager.Instance.ShowPopupUI<UI_TutorialPopup>("UI_TutorialPopup");
    }

    /// <summary>게임오버 — 예전 UI_GameOverPopup이 필드 4개를 직접 대입하던 자리.</summary>
    public void EnterGameOverPhase(GameOverKind kind)
    {
        _data ??= GameDataManager.Instance.Tutorial;

        _data.EnterGameOver(kind);
        _data.SetFlowTime(false);

        if (TutorialPopup != null)
        {
            TutorialPopup.gameObject.SetActive(true);
            TutorialPopup.AdvanceDialog();
        }
    }

    /// <summary>
    /// 꿈 속 출구 근처 — 이동 튜토리얼 단계였다면 한 번만 대사를 띄운다.
    /// Player.Update()에서 매 프레임 발행되므로 ConsumeCurrentStep()이 재발화를 막는다.
    /// </summary>
    private void HandleNearExit()
    {
        if (_data == null || !_data.IsMoveStep) return;

        _data.ConsumeCurrentStep();

        GameManager.Instance.StopGame();

        if (TutorialPopup != null)
        {
            TutorialPopup.gameObject.SetActive(true);
            TutorialPopup.AdvanceDialog();
        }
    }

    /// <summary>꿈에서 탈출 — 지하철 대사로 복귀한다.</summary>
    private void HandleDreamExit()
    {
        if (GameDataManager.Instance.Game.GameMode != GameMode.Tutorial) return;

        _data ??= GameDataManager.Instance.Tutorial;
        _data.SetFlowTime(true);

        if (_data.SubwayIdx < TutorialConfigData.SUBWAY_END_IDX
            && !GameDataManager.Instance.Dream.IsGameOverInDream)
        {
            _data.ReturnToSubway();

            if (TutorialPopup != null)
                TutorialPopup.AdvanceDialog();
        }
    }
}
```

- [ ] **Step 2: 컴파일 검증**

```bash
dotnet msbuild Assembly-CSharp.csproj -t:Build -p:BuildProjectReferences=false -v:m -nologo 2>&1 | grep -c "error CS"
```
Expected: `0`

- [ ] **Step 3: 커밋**

```bash
git add Assets/Scripts/Content/Tutorial/System/
git commit -m "feat: TutorialSystem 추가 — 진행 조정과 Player 이벤트 구독"
```

---

### Task 4: UI_TutorialPopup 전환 ⚠️ 순서 함정 주의

**Files:**
- Modify: `Assets/Scripts/Content/UI/Popup/UI_TutorialPopup.cs`

**Interfaces:**
- Consumes: `GameDataManager.Instance.Tutorial`, `TutorialSystem.Instance.TutorialPopup`, `TutorialData`의 모든 파생 프로퍼티

**🔴 이 Task의 핵심 위험:** 기존 코드는 `SetTutorialTrigger()`로 플래그를 계산한 **뒤** `IncreaseIdx()`를 부르고, 게이트 검사는 **증가 이전 값**을 읽는다. 계산 프로퍼티는 즉시 반영되므로, 증가 **전에** 값을 캡처하지 않으면 튜토리얼 전체가 한 단계씩 밀린다. **컴파일로는 절대 잡히지 않는다.**

- [ ] **Step 1: 데이터 참조 필드 추가**

클래스 상단 필드 선언부에 추가:

```csharp
    private TutorialData _tutorial;
```

- [ ] **Step 2: `Awake`에서 캐싱 + 이벤트 구독**

`Init()` 위에 다음 메서드들을 **새로** 추가한다. 이 클래스에는 현재 `Awake` / `OnEnable` / `OnDisable` / `Start`가 하나도 없다 (`Start`는 `UI_Base`에서 상속받아 `Init()`을 호출한다). `Awake`는 `OnEnable`·`Start`보다 먼저 돌므로 `_tutorial`은 두 곳 모두에서 유효하다.

```csharp
    private void Awake()
    {
        // 순수 C# 데이터 객체이므로 한 번만 캐싱한다 (종료 중 싱글톤 재접근 회피)
        _tutorial = GameDataManager.Instance.Tutorial;
    }

    private void OnEnable()
    {
        if (_tutorial == null) return;

        _tutorial.OnFlowTimeChanged += HandleFlowTimeChanged;
        HandleFlowTimeChanged(_tutorial.StartFlowTime); // 초기 반영
    }

    private void OnDisable()
    {
        if (_tutorial == null) return;

        _tutorial.OnFlowTimeChanged -= HandleFlowTimeChanged;
    }

    private void HandleFlowTimeChanged(bool startFlowTime)
    {
        if (startFlowTime) GameDataManager.Instance.Timer.Resume();
        else               GameDataManager.Instance.Timer.Pause();
    }
```

- [ ] **Step 3: `Update()`를 입력 폴링만 남기고 교체**

기존 `Update()` 전체(`SetTutorialTrigger()` 호출, 11개 OR 체인, `subwayIdx == 8/9` 분기, 매 프레임 `Timer.Pause()/Resume()`)를 아래로 **통째 교체**한다.

```csharp
    private void Update()
    {
        SetTransferText();
        UpdateHintVisibility();

        if (!Input.GetMouseButtonDown(0)) return;

        // ⚠️ 반드시 증가 전에 캡처한다.
        // 예전 코드는 SetTutorialTrigger()가 프레임 시작에 계산한 '증가 이전' 값을 읽었다.
        // 계산 프로퍼티는 즉시 반영되므로 여기서 캡처하지 않으면 단계가 한 칸 밀린다.
        bool wasWaiting = _tutorial.IsWaitingForPlayerAction;

        _tutorial.AdvanceIdx();

        if (wasWaiting)
        {
            GameManager.Instance.ResumeGame();
            gameObject.SetActive(false);
            return;
        }

        AdvanceDialog();
        GameManager.Instance.StopGame();
        gameObject.SetActive(true);
    }

    private void UpdateHintVisibility()
    {
        if (_tutorial.IsTirednessHintStep)
        {
            ShowTirednessUI();
            HideTransferText();
        }
        else if (_tutorial.IsTransferHintStep)
        {
            ShowTransferText();
            HideTirednessUI();
        }
        else
        {
            HideTirednessUI();
            HideTransferText();
        }
    }
```

- [ ] **Step 4: `Init()`의 조건과 팝업 등록을 교체**

```csharp
        // 변경 전: if (!TutorialManager.Instance.isSubwayTutorialEnd || TutorialManager.Instance.isGameoverTutorial)
        if (!_tutorial.IsSubwayEndStep || _tutorial.IsGameOverActive)
        {
            GameManager.Instance.StopGame();
            AdvanceDialog();
        }

        // 변경 전: TutorialManager.Instance.tutorialPopup = this;
        TutorialSystem.Instance.TutorialPopup = this;
```

- [ ] **Step 5: 대사 인덱싱과 Phase 설정을 교체**

`AdvanceDialog()` 계열에서 `TutorialManager.Instance.dialogState = ...` 대입은 **전부 삭제한다** (Phase는 이제 `TutorialData`가 `EnterDream`/`ReturnToSubway`/`EnterGameOver`로만 바꾼다). 인덱스 읽기는 캐싱된 참조로 바꾼다.

```csharp
        // 변경 전: dialog.text = subwayTutorialDialog[TutorialManager.Instance.subwayIdx];
        dialog.text = subwayTutorialDialog[_tutorial.SubwayIdx];
        playerEmotion.sprite = ChangeEmotion(subwayEmotions[_tutorial.SubwayIdx]);

        // dream / gameover 블록도 각각 _tutorial.DreamIdx / _tutorial.GameOverIdx 로 교체
```

`switch (TutorialManager.Instance.dialogState)`는 `switch (_tutorial.Phase)`로 바꾸고 `case TutorialManager.DialogState.Subway:` → `case TutorialPhase.Subway:` 형태로 교체한다. 배열 길이 검사(`< subwayTutorialDialog.Length`)는 그대로 둔다.

- [ ] **Step 6: 컴파일 검증**

```bash
dotnet msbuild Assembly-CSharp.csproj -t:Build -p:BuildProjectReferences=false -v:m -nologo 2>&1 | grep -c "error CS"
```
Expected: `0`

- [ ] **Step 7: 순서 함정 정적 확인**

```bash
grep -n "wasWaiting" Assets/Scripts/Content/UI/Popup/UI_TutorialPopup.cs
```
Expected: `bool wasWaiting = ...` 줄이 `_tutorial.AdvanceIdx();` 줄보다 **위**에 있어야 한다.

- [ ] **Step 8: 커밋**

```bash
git add Assets/Scripts/Content/UI/Popup/UI_TutorialPopup.cs
git commit -m "refactor: UI_TutorialPopup을 TutorialData 구독으로 전환"
```

---

### Task 5: DreamManager / UI_GameOverPopup / UI_GameClearPopup 전환

**Files:**
- Modify: `Assets/Scripts/Content/Dream/DreamManager.cs:34-40`
- Modify: `Assets/Scripts/Content/UI/Popup/UI_GameOverPopup.cs:107-125`
- Modify: `Assets/Scripts/Content/UI/Popup/UI_GameClearPopup.cs:82`

**Interfaces:**
- Consumes: `TutorialSystem.Instance.EnterDreamPhase()`, `EnterGameOverPhase(GameOverKind)`, `GameDataManager.Instance.Tutorial.ConsumeCurrentStep()`

- [ ] **Step 1: `DreamManager` — 필드 4개 대입을 한 줄로**

```csharp
        // 변경 전 (5줄):
        //   TutorialManager.Instance.isSubwayTutorial = false;
        //   TutorialManager.Instance.isDreamTutorial = true;
        //   TutorialManager.Instance.startIncreaseTired = false;
        //   TutorialManager.Instance.dialogState = TutorialManager.DialogState.Dream;
        //   if (TutorialManager.Instance.dreamIdx < TutorialConfigData.EXIT_IDX)
        //       TutorialManager.Instance.tutorialPopup = UIManager.Instance.ShowPopupUI<UI_TutorialPopup>("UI_TutorialPopup");
        TutorialSystem.Instance.EnterDreamPhase();
```

- [ ] **Step 2: `UI_GameOverPopup.GameOverTutorial()` 교체**

```csharp
    private void GameOverTutorial()
    {
        if (GameDataManager.Instance.Game.GameMode != GameMode.Tutorial) return;

        // 현재 도달 가능한 게임오버는 '꿈속 게임오버' 하나뿐이다.
        // 환승 실패 경로는 SubwayData.SetGameOver() 호출부가 없어 죽어 있으므로
        // (스펙 8장), 복구되면 여기서 GameOverKind.Passed로 분기하면 된다.
        TutorialSystem.Instance.EnterGameOverPhase(GameOverKind.Dark);
    }
```

기존의 주석 처리된 `isPassedGameOverTutorial` 블록은 위 주석으로 대체하고 삭제한다.

- [ ] **Step 3: `UI_GameClearPopup.GameClearTutorial()` 교체**

```csharp
        // 변경 전: TutorialManager.Instance.isSubwayTutorialEnd = false;
        // 새 팝업의 Init()이 AdvanceDialog()를 타도록 현재 단계를 소비 처리한다
        GameDataManager.Instance.Tutorial.ConsumeCurrentStep();
        UIManager.Instance.ShowPopupUI<UI_TutorialPopup>("UI_TutorialPopup");
```

- [ ] **Step 4: 컴파일 검증**

```bash
dotnet msbuild Assembly-CSharp.csproj -t:Build -p:BuildProjectReferences=false -v:m -nologo 2>&1 | grep -c "error CS"
```
Expected: `0`

- [ ] **Step 5: 커밋**

```bash
git add Assets/Scripts/Content/Dream/DreamManager.cs Assets/Scripts/Content/UI/Popup/UI_GameOverPopup.cs Assets/Scripts/Content/UI/Popup/UI_GameClearPopup.cs
git commit -m "refactor: 꿈/게임오버/클리어 진입을 TutorialSystem으로 수렴"
```

---

### Task 6: UI_SubwayScene 전환 + TutorialManager 삭제

**Files:**
- Modify: `Assets/Scripts/Content/UI/Scene/UI_SubwayScene.cs` (`StandingTutorial()`, `TutorialButtonBlocker()`)
- Modify: `Assets/Scripts/Utils/ManagerInitializer.cs`
- Modify: `Assets/Scripts/Core/Managers/GameManager.cs`
- Delete: `Assets/Scripts/Content/Tutorial/TutorialManager.cs` + `.meta`

- [ ] **Step 1: `UI_SubwayScene`의 튜토리얼 읽기를 교체**

두 곳 모두 죽은 코드라 동작에는 영향이 없지만, `TutorialManager` 삭제를 위해 컴파일이 통과해야 한다.

```csharp
    // TutorialButtonBlocker() 안
    if (GameDataManager.Instance.Tutorial.IsSlapStep)
        BlockAllButtonsExcept((int)Buttons.SlapButton, (int)Images.SlapFadeImage);
    else if (GameDataManager.Instance.Tutorial.IsStandingStep || GameDataManager.Instance.Tutorial.IsSkipStep)
        BlockAllButtonsExcept((int)Buttons.StandingButton, (int)Images.StandingFadeImage);
```

```csharp
    // StandingTutorial() 안 — 3줄 교체
    GameDataManager.Instance.Tutorial.ConsumeCurrentStep();
    TutorialSystem.Instance.TutorialPopup.gameObject.SetActive(true);
    TutorialSystem.Instance.TutorialPopup.AdvanceDialog();
```

- [ ] **Step 2: `ManagerInitializer` 목록 교체**

```csharp
            DreamManager.Instance,
            TutorialSystem.Instance   // 변경 전: TutorialManager.Instance
```

- [ ] **Step 3: `GameManager.StartDay()`에서 별도 리셋 제거**

```csharp
        // 삭제: TutorialManager.Instance.ResetTutorial();
        // 튜토리얼 상태도 이제 GameDataManager.ResetForNewRun()이 함께 초기화한다
```

- [ ] **Step 4: `TutorialManager` 삭제**

```bash
git rm Assets/Scripts/Content/Tutorial/TutorialManager.cs Assets/Scripts/Content/Tutorial/TutorialManager.cs.meta
```

- [ ] **Step 5: 컴파일 + 잔존 참조 검증**

```bash
dotnet msbuild Assembly-CSharp.csproj -t:Build -p:BuildProjectReferences=false -v:m -nologo 2>&1 | grep -c "error CS"
grep -rn "TutorialManager" Assets/Scripts/ | wc -l
grep -rEn "isSlapTutorial|isStandingTutorial|isSkipTutorial|isEnterDreamTutorial|isSubwayTutorialEnd|isGameClearTutorial|isMoveTutorial|isExitTutorial|isDreamTutorial|isSubwayTutorial|startIncreaseTired|SetTutorialTrigger|dialogState" Assets/Scripts/ | wc -l
```
Expected: 각각 `0`, `0`, `0`

- [ ] **Step 6: 커밋**

```bash
git add -A Assets/Scripts/
git commit -m "refactor: TutorialManager 삭제 — TutorialSystem이 대체"
```

---

### Task 7: 수동 플레이 검증

**Files:** 없음 (검증 전용)

이 Task를 마치기 전에는 어떤 경우에도 "완료"라고 보고하지 않는다. **자동 검증으로는 잡히지 않는 항목들이다.**

- [ ] **Step 1: Unity 에디터에서 컴파일 에러 0 확인**

Console에 에러가 없어야 한다. `TutorialManager`를 참조하던 씬/프리팹이 있었다면 "missing script" 경고가 뜰 수 있으니 확인한다.

- [ ] **Step 2: Day 0 튜토리얼 1회 완주**

StageSelect에서 Day 0을 선택해 아래를 순서대로 확인한다.

1. 지하철 대사가 **한 칸도 밀리지 않고** 진행되는가 — Task 4 순서 함정의 실증
2. 뺨(idx 12) 단계에서 팝업이 사라지고 뺨 버튼만 활성화되는가
3. 입석(idx 16) / 스킵(idx 17) 단계가 정상 동작하는가
4. 피로도 UI(idx 8)와 환승 텍스트(idx 9)가 각 시점에 나타났다 사라지는가
5. 꿈 진입(idx 19) 시 흐름 시간이 재개되는가
6. 꿈속 이동 튜토리얼 후 **출구 근처 대사가 한 번만** 뜨는가 — 승인된 동작 변화의 실증
7. 꿈 탈출 후 지하철 대사로 복귀하는가
8. 꿈속 게임오버 시 게임오버 튜토리얼로 분기하는가
9. 클리어 시 클리어 튜토리얼이 뜨는가

- [ ] **Step 3: 결과 보고**

1~9 중 실패한 항목이 있으면 **그대로 보고한다.** 통과한 항목만 통과라고 적는다.

- [ ] **Step 4: CLAUDE.md 갱신**

`TutorialManager` 관련 서술을 새 구조로 고친다. 최소한 다음 세 곳:

- "Manager Singleton Pattern"의 초기화 순서 목록 — `TutorialManager` → `TutorialSystem`
- "Dialogue & Tutorial" 절 — `TutorialData` / `TutorialSystem` 구조로 재작성
- "Known Gaps"의 `TutorialManager`의 상태가 여전히 public 필드 19개 항목 — **삭제**
- "Key Data Structures" 표에 `TutorialData` 행 추가

```bash
git add CLAUDE.md
git commit -m "docs: CLAUDE.md에 TutorialData/TutorialSystem 구조 반영"
```

---

## 자체 검토 결과

**스펙 커버리지** — 스펙 3장(TutorialData) → Task 1, 4장(TutorialSystem) → Task 3, 5장(호출부 변경표) → Task 2·4·5·6, 6장(순서 함정) → Task 4 Step 3·7, 7장(동작 변화) → Task 7 Step 2의 1·6번, 9장(검증) → 각 Task의 컴파일 단계 + Task 7. 누락 없음.

**타입 일관성** — `AdvanceIdx()` / `ConsumeCurrentStep()` / `EnterDream()` / `ReturnToSubway()` / `EnterGameOver(GameOverKind)` / `SetFlowTime(bool)` / `Reset()`이 Task 1 정의와 Task 3~6 사용처에서 동일. `TutorialSystem.TutorialPopup` 프로퍼티명이 Task 3 정의와 Task 4·6 사용처에서 동일.

**남은 판단 지점** — Task 5 Step 2의 `EnterGameOverPhase` 인자가 현재는 `Dark` 하나뿐이다. `SubwayData.SetGameOver()` 호출부가 없어 `Passed` 경로에 도달할 방법이 없기 때문이며, 스펙 8장에 범위 밖으로 기록된 사항이다.
