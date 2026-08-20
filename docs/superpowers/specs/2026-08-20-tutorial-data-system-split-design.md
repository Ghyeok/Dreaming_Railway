# TutorialManager → TutorialData + TutorialSystem 분리 설계

- 날짜: 2026-08-20
- 브랜치: `refactor/game-data-ownership`
- 상태: 승인됨 (구현 계획 대기)

## 1. 배경

`GameDataManager`로의 데이터 이관은 Subway / Tiredness / Timer / Dream / Game 다섯 도메인에서 끝났고,
튜토리얼만 남았다. `TutorialManager`는 여전히 `public` 필드 19개를 들고 있으며
`DreamManager`, `UI_TutorialPopup`, `UI_GameOverPopup`, `UI_GameClearPopup`, `UI_SubwayScene`이
이 필드를 직접 읽고 쓴다.

CLAUDE.md는 이 작업을 "데이터 이관이 아니라 별도 설계 과제"로 기록해뒀다. 이유는
`is*Tutorial` 플래그들이 **진행 인덱스에서 파생되는 값이면서 동시에 외부에서 꺼서 소비하는 래치**라,
그대로 옮기면 세터만 19개 생기기 때문이다.

### 사전 조사에서 확인된 사실

| 관찰 | 근거 |
|---|---|
| 플래그 8개는 `SetTutorialTrigger()`가 인덱스에서 **매 프레임 재계산**하는 순수 파생값 | `TutorialManager.SetTutorialTrigger()` |
| 필드 3개는 **쓰기만 하고 읽는 곳이 0** | `startIncreaseTired`(6/0), `isSubwayTutorial`(4/0), `isDreamTutorial`(4/0) |
| 메서드 3개는 **호출부가 0** | `LoadTutorialOverlay()`, `LoadScriptOverlay()`, `StopIncreaseTired()` |
| 외부의 `= false` 래치는 **같은 콜스택 안에서만 유효** — 다음 `SetTutorialTrigger()`가 되살림 | `HandleNearExit`, `UI_GameClearPopup:82` |
| `Player.OnNearExit`은 `Player.Update()`에서 발행되고 `Update`는 `timeScale = 0`에서도 돌므로 **출구 근처에서 매 프레임 발화** | `Player.cs:63, 198` |
| 튜토리얼 ⟺ Day 0 ⟺ `GameMode.Tutorial` — 단일 진입점에서 함께 설정 | `UI_StageSelectScene.OnStageClicked(0)` |

## 2. 승인된 결정

1. **파생값 접기** — 파생 플래그는 `TutorialData`의 getter-only 계산 프로퍼티가 되고
   `SetTutorialTrigger()`는 삭제한다. 기계적 이관(세터 19개)도, 전면 상태기계 재설계도 아니다.
2. **`TutorialManager` 삭제** — `TutorialSystem`이 그 자리를 대체한다.
   Subway 도메인에 `SubwayManager`가 없는 것과 같은 모양.

## 3. TutorialData (순수 C#)

`GameDataManager.Tutorial`이 소유한다. `MonoBehaviour` 아님, 싱글톤 아님, 매니저 참조 없음.
파일 위치: `Assets/Scripts/Content/Tutorial/Data/TutorialData.cs`

```csharp
public enum TutorialPhase { Subway, Dream, GameOver }
public enum GameOverKind  { Dark, Passed }
```

### 3.1 진짜 상태 (8개)

| 프로퍼티 | 대체하는 기존 필드 |
|---|---|
| `Phase` | `dialogState` |
| `SubwayIdx` / `DreamIdx` / `GameOverIdx` | 동명 필드 |
| `StartFlowTime` | 동명 필드 |
| `IsGameOverActive` | `isGameoverTutorial` |
| `OverKind` | `isDarkGameOverTutorial` + `isPassedGameOverTutorial` |
| `_consumedIdx` (private 필드) | 외부의 `= false` 래치 |

모두 `private set`. 변경은 전용 메서드로만:

| 메서드 | 하는 일 | 대체하는 기존 코드 |
|---|---|---|
| `AdvanceIdx()` | 현재 Phase의 인덱스 +1, `_consumedIdx` 무효화, `OnStepChanged` 발행 | `IncreaseIdx()` |
| `EnterDream()` | `Phase = Dream`, `_consumedIdx = -1` | `DreamManager:34-37` |
| `EnterGameOver(kind)` | `Phase = GameOver`, `IsGameOverActive = true`, `OverKind = kind`, `GameOverIdx = kind별 시작값`, `_consumedIdx = -1` | `UI_GameOverPopup:107-114` |
| `ReturnToSubway()` | `Phase = Subway`, `_consumedIdx = -1` | `HandleDreamExit`의 복귀 블록 |
| `SetFlowTime(bool)` | `StartFlowTime` 갱신 + 값이 바뀔 때만 `OnFlowTimeChanged` 발행 | `startFlowTime = ...` 3곳 |
| `ConsumeCurrentStep()` | `_consumedIdx = CurrentIdx` | 외부의 `is*Tutorial = false` |
| `Reset()` | 전체 초기화 | `ResetTutorial()` |

> Phase를 바꾸는 세 메서드(`EnterDream` / `EnterGameOver` / `ReturnToSubway`)는
> 모두 `_consumedIdx`를 초기화한다 — 3.2의 오탐 방지 조건.

### 3.2 파생 프로퍼티 (getter-only, 세터 없음)

```csharp
// 현재 Phase가 가리키는 인덱스
private int CurrentIdx => Phase switch
{
    TutorialPhase.Subway => SubwayIdx,
    TutorialPhase.Dream  => DreamIdx,
    _                    => GameOverIdx,
};

// 인덱스가 바뀌면 자동 무효화.
// ⚠️ Phase가 바뀔 때도 반드시 _consumedIdx = -1로 초기화해야 한다.
//    Dream에서 3을 소비한 뒤 Subway로 돌아와 SubwayIdx가 3이면 오탐이 나기 때문.
private bool IsConsumed => _consumedIdx == CurrentIdx;

public bool IsSlapStep          => Phase == Subway && SubwayIdx == SLAP_IDX          && !IsConsumed;
public bool IsStandingStep      => Phase == Subway && SubwayIdx == STANDING_IDX      && !IsConsumed;
public bool IsSkipStep          => Phase == Subway && SubwayIdx == SKIP_IDX          && !IsConsumed;
public bool IsEnterDreamStep    => Phase == Subway && SubwayIdx == ENTER_DREAM_IDX   && !IsConsumed;
public bool IsSubwayEndStep     => Phase == Subway && SubwayIdx == SUBWAY_END_IDX    && !IsConsumed;
public bool IsGameClearStep     => Phase == Subway && SubwayIdx == GAME_CLEAR_IDX    && !IsConsumed;
public bool IsMoveStep          => Phase == Dream  && DreamIdx  == MOVE_IDX          && !IsConsumed;
public bool IsExitStep          => Phase == Dream  && DreamIdx  == EXIT_IDX          && !IsConsumed;

// 구 매직넘버 8 / 9 — TutorialConfigData 상수로 승격
public bool IsTirednessHintStep => Phase == Subway && SubwayIdx == TIREDNESS_HINT_IDX;
public bool IsTransferHintStep  => Phase == Subway && SubwayIdx == TRANSFER_HINT_IDX;

// UI_TutorialPopup.Update()의 11개 OR 체인을 대체
public bool IsWaitingForPlayerAction =>
    IsSlapStep || IsStandingStep || IsSkipStep || IsEnterDreamStep ||
    IsSubwayEndStep || IsGameClearStep || IsMoveStep || IsExitStep ||
    (IsGameOverActive && Phase == GameOver);
```

`SetTutorialTrigger()`는 **삭제된다.** 매 프레임 플래그 8개를 재계산하던 함수가 사라지는 것이
이번 작업의 핵심 수확이다.

### 3.3 이벤트

```csharp
public event Action OnStepChanged;           // Phase 또는 인덱스 변경 시
public event Action<bool> OnFlowTimeChanged; // Timer Pause/Resume 구동
```

`OnFlowTimeChanged`는 `UI_TutorialPopup.Update()`가 매 프레임 `Timer.Pause()/Resume()`을
호출하던 것을 대체한다.

### 3.4 삭제되는 필드 (읽는 곳 0)

`startIncreaseTired`, `isSubwayTutorial`, `isDreamTutorial` — 동작 변화 없음.

## 4. TutorialSystem

파일 위치: `Assets/Scripts/Content/Tutorial/System/TutorialSystem.cs`

```csharp
public class TutorialSystem : SingletonManagers<TutorialSystem>, IManager
{
    private TutorialData _data;
    public UI_TutorialPopup TutorialPopup { get; set; }   // 뷰 참조는 데이터가 아니라 여기

    public void Init() { _data = GameDataManager.Instance.Tutorial; }

    private void OnEnable()  { Player.OnNearExit += HandleNearExit; Player.OnDreamExit += HandleDreamExit; }
    private void OnDisable() { Player.OnNearExit -= HandleNearExit; Player.OnDreamExit -= HandleDreamExit; }

    public void EnterDreamPhase();                    // 구 DreamManager 5줄
    public void EnterGameOverPhase(GameOverKind kind);// 구 UI_GameOverPopup 5줄
    private void HandleNearExit();
    private void HandleDreamExit();
}
```

`ManagerInitializer` 목록에서 `TutorialManager` → `TutorialSystem`으로 **교체**한다.
`Player.OnNearExit`이 `static` 이벤트라 **꿈 씬이 로드되기 전에 구독돼 있어야 하므로**
목록에서 빼면 안 된다 (`DreamManager`와 같은 이유).

> `TimerSystem` / `SubwayFlowSystem`은 `SubwaySceneDirector.Start()`가 `Init()`을 부르지만,
> `TutorialSystem`은 꿈 씬에서도 살아 있어야 하고 지하철 씬 진입 전부터 구독이 필요하므로
> `ManagerInitializer`가 `Init()`을 부른다. 이 차이는 의도된 것이다.

## 5. 호출부 변경

| 파일 | 현재 | 변경 후 |
|---|---|---|
| `GameManager.cs:26` | `TutorialManager.Instance.ResetTutorial()` 별도 호출 | 삭제 — `GameDataManager.ResetForNewRun()`에 `_tutorial.Reset()` 합류 |
| `DreamManager.cs:34-40` | 필드 4개 직접 대입 + 팝업 생성 | `TutorialSystem.Instance.EnterDreamPhase()` |
| `UI_GameOverPopup.cs:107-125` | 필드 4개 직접 대입 + 팝업 조작 | `TutorialSystem.Instance.EnterGameOverPhase(GameOverKind.Dark)` |
| `UI_GameClearPopup.cs:82` | `isSubwayTutorialEnd = false` | `Tutorial.ConsumeCurrentStep()` |
| `UI_TutorialPopup.Update()` | `SetTutorialTrigger()` + 11개 OR + Timer Pause/Resume 매 프레임 | `IsWaitingForPlayerAction` 1개 + `OnFlowTimeChanged` 구독. **입력 폴링만 남음** |
| `UI_TutorialPopup` 대사 인덱싱 | `subwayTutorialDialog[TutorialManager.Instance.subwayIdx]` | `_tutorial.SubwayIdx` (캐싱된 데이터 참조) |
| `UI_SubwayScene.cs:293-295` | `isSlapTutorial` 등 읽기 | `IsSlapStep` 등 (죽은 코드라 동작 무관) |
| `ManagerInitializer.cs:17` | `TutorialManager.Instance` | `TutorialSystem.Instance` |

## 6. ⚠️ 구현 시 반드시 지킬 것 — 인덱스 증가와 게이트 검사의 순서

현재 `UI_TutorialPopup.Update()`는 이 순서로 돈다:

```csharp
SetTutorialTrigger();                 // (1) 플래그가 subwayIdx = k 기준으로 계산됨
...
if (Input.GetMouseButtonDown(0))
{
    IncreaseIdx();                    // (2) subwayIdx = k+1
    if (isSlapTutorial || ...)        // (3) 여전히 k 기준 값을 읽음
}
```

즉 **게이트 검사는 증가 이전(k) 값을 본다.** 계산 프로퍼티로 바꾸면 (3)이 자동으로 k+1을 보게 되어
단계 게이팅이 한 칸 어긋난다. 반드시 증가 전에 값을 캡처해야 한다:

```csharp
bool wasWaiting = _tutorial.IsWaitingForPlayerAction;   // 증가 전 캡처
_tutorial.AdvanceIdx();
if (wasWaiting) { GameManager.Instance.ResumeGame(); gameObject.SetActive(false); return; }
```

이 한 줄을 놓치면 튜토리얼 전체가 한 단계씩 밀린다. 컴파일로는 절대 잡히지 않는다.

## 7. 승인된 동작 변화

1. **`HandleNearExit`가 매 프레임 → 1회 발화.** 현재는 출구 근처에 서 있는 동안
   `StopGame()` + `AdvanceDialog()`가 매 프레임 반복 호출된다(같은 대사라 눈에 띄지 않을 뿐).
   `ConsumeCurrentStep()` 도입으로 1회가 된다.
2. **파생값의 1프레임 지연 소멸.** 현재는 인덱스가 바뀌어도 다음 `SetTutorialTrigger()` 전까지
   플래그가 옛 값이다. 계산 프로퍼티는 즉시 반영된다.
3. **필드 3개 삭제** — 읽는 곳이 0이라 동작 변화 없음.

## 8. 범위 밖 — 조사 중 발견했으나 이번에 고치지 않는 것

- **`AdvanceState()` 회귀 (커밋 안 된 작업분).** `isFinalStation`이 노선 진행 판정에까지 쓰여
  `GoToNextLine()`이 마지막 노선에서만 호출된다. **튜토리얼은 Day 0이라
  `MaxTransferCount = 1` → `CurTransferCount == 0 == MaxTransferCount - 1`이 항상 참이므로
  영향받지 않는다.** 노말 모드(Day ≥ 1)와 무한 모드만 깨진다. 별도 수정 대상.
- **튜토리얼이 노선 999개를 생성한다.** `StationSystem.GetLineCount()`가
  `_isNormalMode ? CurrentDay + 1 : 999`라 `GameMode.Tutorial`은 999를 받는데,
  `DetermineMaxTransferCount()`는 Day 0에 1을 준다. 남는 998개는 쓰이지 않지만
  약 2만 개의 `StationData`가 튜토리얼 시작마다 할당된다.
- **`isPassedGameOverTutorial` 경로.** 설정하는 코드가 전부 주석 처리돼 있어 항상 `false`다.
  `SubwayData.SetGameOver()` 호출부가 없는 기존 Known Gap과 같은 뿌리.
  `GameOverKind.Passed`는 정의만 해두고 실제 진입 경로는 만들지 않는다.

## 9. 검증 계획

이 프로젝트에는 튜토리얼 자동 테스트가 없다. 검증은 두 층으로 나뉜다.

**자동으로 보장 가능한 것**

```bash
dotnet msbuild Assembly-CSharp.csproj -t:Build -p:BuildProjectReferences=false
```

- 컴파일 에러 0
- `grep -rn "TutorialManager" Assets/Scripts/` 결과 0건 (전수 교체 확인)
- 삭제 대상 필드/메서드의 잔존 참조 0건

**수동 플레이로만 확인 가능한 것 (필수)**

Day 0을 선택해 튜토리얼을 처음부터 끝까지 1회 플레이하며 확인한다.

1. 지하철 대사가 한 칸도 밀리지 않고 진행되는가 (6장 순서 문제의 실증)
2. 뺨 / 입석 / 스킵 단계에서 팝업이 사라지고 해당 버튼만 활성화되는가
3. 피로도 UI(idx 8)와 환승 텍스트(idx 9)가 각 시점에 나타나는가
4. 꿈 진입 후 이동 튜토리얼 → 출구 근처 대사가 **한 번만** 뜨는가 (7장 1번의 실증)
5. 꿈 탈출 후 지하철 대사로 복귀하는가
6. 꿈속 게임오버 시 게임오버 튜토리얼로 분기하는가
7. 클리어 시 클리어 튜토리얼이 뜨는가

구현자는 1~7을 직접 수행하기 전에는 "완료"라고 보고하지 않는다.
