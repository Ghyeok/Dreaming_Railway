# AGENTS.md

This file provides guidance to coding agents when working with code in this repository.

## Project Overview

**Dreaming Railway** is a 2D side-scrolling Unity game (URP, Unity 6.3 / 6000.3.8f1) with a dual-world mechanic. The player rides a subway, fights off sleep by slapping themselves, and when tiredness maxes out they fall into a dream platformer. The goal is to survive all transfer stations across 5 days (plus an infinite mode).

**Scenes**: MainScene → StageSelectScene → SubwayScene ↔ DreamScene (ScriptScene and TutorialScene overlay as additive loads)

## Unity / Build

There are no custom build scripts and no `.asmdef` files — everything compiles into a single `Assembly-CSharp`. Open the project in Unity 6.3 and build via **File > Build Settings**. The target resolution is 1920×1080. Key packages: URP, TextMesh Pro, New Input System.

To type-check without opening the editor, build the Unity-generated project (it is gitignored and regenerated on refresh):

```bash
dotnet msbuild Assembly-CSharp.csproj -t:Build -p:BuildProjectReferences=false
```

> This requires Unity to have compiled at least once in this session — the csproj references package DLLs from `Temp/bin/Debug/`, which Unity clears on exit. If they are missing, copy them from `Library/ScriptAssemblies/` first.

Because there is no asmdef split, **editor-only namespaces silently compile in the editor but break player builds.** Never `using UnityEditor.*` (including package editor namespaces like `UnityEditor.ShaderGraph.Internal`) from a runtime script.

## Architecture

### Manager Singleton Pattern

All global managers inherit `SingletonManagers<T>` (`Assets/Scripts/Core/Managers/Base/`), which handles `DontDestroyOnLoad` and auto-creation. Managers implement `IManager` with an `Init()` method.

Initialization order (driven by `ManagerInitializer`, at `RuntimeInitializeOnLoadMethod(BeforeSceneLoad)`):
```
GameManager → GameDataManager → SceneTransitionManager → SoundManager → UIManager → DreamManager → TutorialSystem
```

Access pattern everywhere: `GameManager.Instance`, `UIManager.Instance`, etc.

이 리스트에는 **싱글톤 매니저만** 넣는다. `GameDataManager`는 `Init()`가 비어 있지만 모든 데이터의 소유자라 누가 읽기 전에 존재해야 하므로 목록 앞쪽에 둔다.

`TimerSystem`과 `SubwayFlowSystem`은 매니저가 아니라 **이 목록에 없다.** 둘 다 `SubwaySceneDirector.Start()`가 `Init()`을 부른다 — 지하철 씬에 들어올 때마다. 단 구현은 여전히 `SingletonManagers<T>`라 GameObject는 `DontDestroyOnLoad`로 살아남고, 꿈 씬에서도 계속 틱한다 (싱글톤을 떼면 지하철 흐름이 꿈에서 멈춰 핵심 메커닉이 깨진다).

> `SingletonManagers<T>.Instance` returns **null** after `OnDestroy`/`OnApplicationQuit` (`_isShuttingDown`). Never call `.Instance` from `OnDisable`/`OnDestroy` — cache the reference in a field during `Awake`/`Init` and unsubscribe through that field.

### Data / Logic Separation — `GameDataManager` as the single entry point

All gameplay **data** lives in plain C# classes owned by `GameDataManager`; the **logic** lives in separate `*System` MonoBehaviours. Nothing else owns game state.

```
GameDataManager
  ├─ Game      : GameData           // 게임 모드 + 현재 Day + 최고 클리어 Day
  ├─ Tiredness : TirednessData      // 피로도
  ├─ Timer     : TimerData          // 플레이 타임 + 전역 일시정지
  ├─ Dream     : DreamData          // 꿈 진입 여부 + 꿈속 게임오버
  └─ Subway    : SubwayData         // 노선 + 진행 + 환승 + 뺨/입석/게임오버
                   └─ List<LineData> → List<StationData>
```

The rules:

- **Data classes** (`GameData`, `TirednessData`, `SubwayData`, `TimerData`, `DreamData`) are pure C# — no `MonoBehaviour`, no singleton. State is `private set`; all mutation goes through explicit methods. **Events live on the data class**, not on the system.
- **Data instances are created only by `GameDataManager`.** Never `new TirednessData()` / `new SubwayData()` elsewhere — a second instance produces silently divergent state rather than a loud failure.
- **System classes** (`TirednessSystem`, `SubwayFlowSystem`, `StationSystem`, `TimerSystem`) hold no game state. They cache their data in `Init()` via `GameDataManager.Instance.<X>` and drive it from `Update()`.
- **Readers** (UI, background scrollers) subscribe to the data's events and read the data directly — they never go through a system.
- Data classes must not reference `GameManager`/`UIManager`. Values that depend on them (e.g. `MaxTransferCount`) are computed by a system and passed in as a parameter.
- `GameDataManager.Instance` may be read at initialization time, but **cache the result in a field** — do not re-read it per frame or during teardown.
- **런 단위 리셋은 `GameDataManager.ResetForNewRun()` 하나로 수렴한다.** 각 데이터의 `Reset()`을 개별 호출하지 말 것. 호출 지점은 두 곳 — `GameManager.StartDay()`(튜토리얼+노말)와 `UI_MainScene.InfiniteModeOnClicked()`(무한 모드는 `StartDay()`를 타지 않는다). 튜토리얼 진행 상태도 이제 `TutorialData`라 `ResetForNewRun()`이 `_tutorial.Reset()`을 다른 데이터와 나란히 부른다 — 예전처럼 매니저를 따로 거치지 않는다.

Example:

```csharp
// System — 상태 없음, 데이터를 당겨와 구동만 한다
public void Init() { _data = GameDataManager.Instance.Subway; }
private void Update() { _data.Tick(Time.deltaTime); }

// Reader — Awake에서 한 번 캐싱하고, 데이터 이벤트에 직접 구독
private void Awake()    { _tiredness = GameDataManager.Instance.Tiredness; }
private void OnEnable() { _tiredness.OnTiredChange += UpdateFill; }
private void OnDisable(){ _tiredness.OnTiredChange -= UpdateFill; }
```

### Timer (TimerData + TimerSystem) — Global Pause & Play Time

`TimerData` owns `PlayTime` (shown on the GameOver/GameClear results screens) and the global pause flag `IsPaused`, mutated via `Pause()` / `Resume()`. `TimerSystem` is a `DontDestroyOnLoad` singleton that only calls `_data.Tick(Time.deltaTime)` — time must keep running across scene transitions, same as `SubwayFlowSystem`. `Init()`는 `SubwaySceneDirector.Start()`가 부르며 데이터 캐싱만 한다 — `PlayTime` 리셋은 `GameDataManager.ResetForNewRun()` 한 곳에서만 일어난다. 꿈에서 돌아와 지하철 씬이 다시 올라와도 누적 시간이 날아가지 않는 이유다.

Per-frame gameplay systems (`SubwayFlowSystem`, `TirednessSystem`) each run their own `Update()` and guard it with `if (_timer == null || _timer.IsPaused) return;`, where `_timer` is cached from `GameDataManager.Instance.Timer` in `Init()`. There is no central tick-dispatch interface. `Pause()` is used specifically where a popup (GameOver/GameClear) needs gameplay frozen without touching `Time.timeScale` (which would also freeze the popup's own fade-in). Full freezes that should also stop animations/coroutines use `GameManager.StopGame()/ResumeGame()` (`Time.timeScale = 0`) instead.

> 일시정지 축이 세 개 있다: `TimerData.IsPaused`(전역, timeScale 무관), `GameManager.IsGameStopped`(`Time.timeScale = 0`), `TirednessData.IsPaused`(입석 전용). 앞의 두 개를 하나로 합치는 작업은 아직 하지 않았다.

### GameManager — 수명주기 조정 (상태는 GameData가 소유)

진행 값은 `GameData`(`GameDataManager.Game`)가 갖는다 — `GameMode`(None / Tutorial / NormalMode / InfiniteMode), `CurrentDay`(1–5), `MaxClearDay`. 모드 변경은 매니저를 거치지 말고 `GameDataManager.Instance.Game.SetGameMode(mode)`를 직접 호출한다.

`GameManager`에 남은 것은 **데이터 변경만으로 끝나지 않는 일**뿐이다:

- `StartDay(day)` — `CurrentDay` 설정 + `Dream.Reset()` (새 런 시작)
- `UpdateMaxClearDay()` — `GameData.TryUpdateMaxClearDay()`가 true를 반환할 때만 `SaveManager`에 저장. 데이터 클래스가 매니저를 참조하지 않도록 **판단은 데이터가, 저장은 매니저가** 한다.
- `StopGame()` / `ResumeGame()` / `IsGameStopped` — `Time.timeScale = 0`과 한 몸이라 순수 데이터가 아니다. 그래서 `GameData`로 옮기지 않았다.

> `MaxClearDay`는 `GameManager.Init()`에서 PlayerPrefs로부터 한 번 로드되며, 그 뒤로 읽는 쪽(`UI_MainScene`, `UI_StageSelectScene`)은 `SaveManager`를 직접 호출하지 않고 `Game.MaxClearDay`를 읽는다 — 예전에는 두 경로가 공존했다.

### SubwaySceneDirector — 씬 부트스트랩 + 진행 조정

`SubwaySceneDirector` (execution order −100) has two lifetimes, not one:

1. **`Start()` 1회 — 씬 부트스트랩** — 각 시스템의 `Init()`을 올바른 순서로 호출하고, BGM 재생·`SetFlowSpeed(false)` 같은 씬 진입 1회성 로직을 실행
2. **씬이 유지되는 동안 — 진행 조정** — 꿈씬 진입 3경로를 단일 지점으로 수렴시키고(중복 진입 가드 포함), 데이터 이벤트를 받아 팝업을 띄움

> 이 클래스는 **DI 컨테이너가 아니다.** 시스템 참조를 다른 컴포넌트에 코드로 중계하지 않는다. 데이터가 필요한 쪽은 `GameDataManager`에서 직접 가져오고, 컴포넌트 참조가 필요하면 인스펙터의 `[SerializeField]`로 잇는다. Director가 코드로 보장하는 것은 **`Init()` 호출 순서**뿐이다 — 인스펙터로는 표현할 수 없기 때문.

```csharp
// Start()에서 게임 데이터를 캐싱한 뒤 순서대로 초기화
_subwayData = GameDataManager.Instance.Subway;
_tirednessData = GameDataManager.Instance.Tiredness;

TimerSystem.Instance.Init();       // 씬을 넘는 전역 시스템도 여기서 물린다
SubwayFlowSystem.Instance.Init();

_station.Init();   // 노선이 없으면 생성 → _subway.BeginRun()
_tiredness.Init();
subwayPlayerContext.Init();

// 꿈속 진입 이벤트를 단일 지점에서 처리 (_isEnteringDream으로 중복 진입 차단)
_tirednessData.OnTiredMaxed += MoveToDreamScene;
subwayPlayerContext.OnFellAsleep += MoveToDreamScene;   // 바로 잠들기
subwayPlayerContext.OnSkipped += MoveToDreamScene;      // 입석 후 스킵

// 팝업 표시도 Director가 담당 (데이터는 이벤트만 발행)
_subwayData.OnDayCleared += OnDayCleared;
_subwayData.OnSubwayGameOver += OnSubwayGameOver;
```

`MoveToDreamScene()`은 진입 전처리(피로도 조정, 흐름 속도 변경)와 `SceneTransitionManager.Instance.GoToDream()` 호출을 순서대로 담당한다. `Start()`의 `SetFlowSpeed(false)`가 꿈에서 걸린 3–4배속을 되돌리는 **유일한 지점**이므로 제거하면 안 된다.

> ⚠️ **프리팹 안의 컴포넌트에 씬 오브젝트 참조를 `[SerializeField]`로 물리지 말 것.** `SubwayPlayer`는 `UI_SubwayScene.prefab` 소속이라, 씬 오브젝트를 가리키는 필드는 프리팹 애셋에 저장될 수 없고 프리팹 인스턴스 override로만 남는다 — Revert 한 번이나 다른 씬 배치에서 조용히 null이 된다. 그런 의존은 만들지 말고 데이터를 직접 쓰도록 설계할 것.

### Tiredness (TirednessData + TirednessSystem)

`TirednessData.Tick()` accumulates `CurrentTiredness += deltaTime` and fires `OnTiredChange` every tick, `OnTiredMaxed` at 100. On dream entry, `ApplyDreamEnterRecovery(awakeTime)` halves tiredness if awake time ≤ 100 s, otherwise scales it by 2/3.

`IsPaused`도 **데이터가 소유한다.** 입석 시 `SetForced(STANDING_TIREDNESS)`가 값을 99.9로 고정하면서 증가를 멈추고 (`Tick()`이 `IsPaused`에서 조기 return), `TirednessSystem.Init()`의 `Resume()`이 지하철 씬 재진입 때 해제한다. 99.9인 이유는 100 미만이어야 `OnTiredMaxed`로 인한 자동 꿈 진입이 걸리지 않기 때문.

`TirednessSystem`은 틱 구동 전용이다 — 상태도, 값 변경 래퍼도 갖지 않는다. **값을 바꾸는 쪽은 시스템을 거치지 말고 `TirednessData`를 직접 호출한다** (`SubwayPlayer`가 `_tiredness.Decrease()` / `_tiredness.SetForced()`를 직접 부르는 식). 시스템 참조를 주입하면 위의 프리팹 제약에 걸린다.

### Subway Flow (SubwayData + SubwayFlowSystem)

`SubwayData.Tick()` is the state machine. It alternates between travelling (`IsSubwayStopping = false`) and stopped at a station (`IsSubwayStopping = true`), driven by `TimeToNextState` counting down at `FlowSpeed` (1× normally, 3.1–4.1× while in the dream scene). `Tick()` no-ops until `HasLines` is true, so it is safe for it to run before `StationSystem` has generated the lines.

`SubwayFlowSystem` is a **`DontDestroyOnLoad` singleton, not a scene object** — the subway must keep ticking while the player is in DreamScene, which is the core mechanic. It holds no state; it only calls `_data.Tick()` / `_data.TickSlapCooldown()` and forwards `OnDayCleared` to `GameManager.UpdateMaxClearDay()` (that must happen even when the day is cleared mid-dream, where `SubwaySceneDirector` doesn't exist). `Init()` 호출은 `SubwaySceneDirector.Start()`가 맡고, 재진입 시 중복 구독은 `Init()` 안에서 막는다.

`StationSystem` generates the line data once per run (travel time 10–15 s, stop time 6–8 s, 20 stations per line) and hands it over with `_subway.BeginRun(lines, maxTransferCount)`. It skips generation entirely when `_subway.HasLines` is already true, which is how state survives the DreamScene round trip. `Init()`은 그 조기 return **앞에서** `_subway.ResetPlayerSession()`을 부른다 — 뺨/입석 세션 값은 노선 재생성 여부와 무관하게 지하철 씬 진입마다 초기화돼야 하기 때문. Transfer count: `day + 1` in normal mode, 999 in infinite mode.

> `IsRunFinished` is set when the last transfer completes. It is required: without it `Tick()`'s `while` loop would index past the end of `SubwayLines` forever and freeze the editor.

### Subway Rules (in SubwayData)

The former `SubwayRuleManager` is gone; its rules now live as data + `SubwayPlayer` actions:
- **Slap**: `SubwayPlayer.TrySlap()` → `SubwayData.StartSlapCooldown()`, reduces tiredness by `TiredDecreaseBySlap` (3 normal / 4 infinite) with a 5-second cooldown ticked by `SubwayFlowSystem.Update()` → `TickSlapCooldown()`.
- **Standing**: `TryStand()` forces tiredness to 99.9; `TrySkip()` calls `SubwayData.ForceTransferByStanding()`. After standing, the player cannot stand again until 2 more lines end — `AdvanceState()`가 `OnLineEnded` 발행 직전에 `AddStandingCount()`를 직접 부른다. **`ForceTransferByStanding()`은 이 경로를 타지 않는다** (스킵한 그 자리에서 쿨다운이 한 칸 새면 안 되므로).
- **Game over**: `SubwayData.SetGameOver()` fires `OnSubwayGameOver`; `SubwaySceneDirector` shows the popup. ⚠️ Nothing currently calls `SetGameOver()` — the missed-transfer detection was lost when `SubwayRuleManager` was deleted, so the subway game-over path is dead.

### SubwayPlayer — UI 반응 행동 + 애니메이션

FSM은 완전히 사라졌다. `PlayerState` enum / `ChangeState()` / `OnStateChanged`는 없다. `SubwayPlayer`가 하는 일은 두 가지뿐이다:

1. **UI 입력에 반응하는 행동** — 각 메서드가 `규칙 판정 → 데이터 변경 → SFX → 애니메이션 → 이벤트`를 직선으로 수행한다.

| 메서드 | 데이터 변경 | 애니메이터 | 이벤트 | 구독자 |
|---|---|---|---|---|
| `TrySlap()` | `StartSlapCooldown()` + `Tiredness.Decrease()` | `isSlap` | `OnSlapSuccessed` | `UI_SubwayScene` (쿨타임 게이지 / 횟수) |
| `TryStand()` | `Tiredness.SetForced(99.9)` | `isStanding` | `OnStood` | `UI_SubwayScene` (입석 → 스킵 버튼 교체) |
| `TrySkip()` | `StartStandingCooldown()` + `ForceTransferByStanding()` | `isSkip` | `OnSkipped` | `SubwaySceneDirector` (꿈 진입) |
| `TryFallAsleep()` | 없음 | `isFallAsleep` | `OnFellAsleep` | `SubwaySceneDirector` (꿈 진입) |
| `TryTransfer()` | 없음 | `isTransfer` | 없음 | ⚠️ 현재 호출부 없음 |

2. **피로도에 따른 애니메이션** — `TirednessData.OnTiredChange`를 구독해 `isSleeping` bool을 `IsTiredHalf`로 갱신한다.

> `_isPoseLocked` bool 하나가 FSM이 하던 게이팅을 대신한다. 입석/잠들기로 포즈를 잡으면 true가 되고, 그 뒤로는 피로도 변화가 `isSleeping`을 건드리지 못한다 — 입석은 피로도를 99.9로 **고정**하므로 이 잠금이 없으면 `OnTiredChange`가 곧바로 입석 포즈를 수면 포즈로 덮어쓴다. 상태기계가 아니라 애니메이션 포즈 잠금 플래그다.

`SubwayPlayer`는 게임 데이터를 **소유하지도, 중계하지도 않는다.** 예전의 `public SubwayData Data` 패스스루는 사라졌고, UI는 `GameDataManager.Instance.Subway`를 직접 읽는다. 런 세션 초기화(`ResetPlayerSession()`)는 `StationSystem.Init()`이, 뺨 쿨타임 틱은 `SubwayFlowSystem.Update()`가 담당한다. `AnimationEventHandler` (`Assets/Scripts/Content/Player/AnimationEventHanlder.cs`) bridges Unity animation events to player callbacks.

### UI Framework

All UI inherits from `UI_Base` (`Assets/Scripts/Content/UI/Base/`). The binding system maps child GameObjects to enum-named entries:

```csharp
Bind<Button>(typeof(Buttons));      // binds by enum name matching GameObject name
Button btn = GetButton((int)Buttons.StartButton);
```

`UI_Popup` is stack-managed by `UIManager` (`ShowPopupUI<T>(name)` / `ClosePopupUI()`). `UI_Scene` is a single persistent overlay per scene (`ShowSceneUI<T>(name)`). All UI prefabs live under `Resources/Prefabs/UIs/`.

### Dream (DreamData + DreamManager)

꿈 상태는 `DreamData`(`GameDataManager.Dream`)가 소유한다 — `IsInDream`, `IsGameOverInDream`. `DreamManager`(`Assets/Scripts/Content/Dream/DreamManager.cs`)는 상태를 갖지 않고 진입만 조정한다: `SceneManager.sceneLoaded`를 구독해 꿈 씬 진입 시 `EnterDream()`을 호출하고, 튜토리얼 모드에서는 `TutorialSystem.Instance.EnterDreamPhase()`를 불러 진행을 게이트한다.

> ⚠️ **`DreamManager`는 `ManagerInitializer` 목록에 반드시 있어야 한다.** 꿈 씬이 로드되기 *전에* 생성돼 `sceneLoaded`를 구독하고 있어야 하기 때문 — 빠지면 `IsInDream`이 영영 false로 남아 일시정지/설정/노선 팝업이 조용히 오작동한다.

`IsGameOverInDream`은 새 런 시작 시 `GameDataManager.ResetForNewRun()`이 부르는 `DreamData.Reset()`으로 해제된다.

뺨 횟수는 `DreamData`에 두지 않는다 — 맵 스포너(`MapXSpawn`, `MapYSpawn`)가 `GameDataManager.Instance.Subway.SlapNum`을 직접 읽는다. 예전에는 꿈 진입 시 복사본을 만들었는데, 원본과 조용히 어긋날 수 있어 제거했다.

`DreamSceneManager` is the dream-side counterpart to `SubwaySceneDirector` — it controls dream init order and calls `Subway.SetFlowSpeed(true)`. `FogOrigin` centralizes the fog/map/camera direction decision.

### Dialogue & Tutorial

**ScriptManager** (`Assets/Scripts/Content/Dialogue/ScriptManager.cs`): Loads per-day story text as `DialogLine` structs (Text + Emotion fields). Shows `UI_ScriptPopup` at day start and after clearing. Emotion sprites are loaded from `Resources/`.

**TutorialData** (`Content/Tutorial/Data/TutorialData.cs`, owned by `GameDataManager.Tutorial`): pure C# progress state — `Phase`(`TutorialPhase.Subway/Dream/GameOver`) + `SubwayIdx`/`DreamIdx`/`GameOverIdx`. 각 단계 트리거는 `is*Step`류 getter-only 프로퍼티(`IsSlapStep`, `IsStandingStep`, `IsSkipStep`, `IsEnterDreamStep`, `IsSubwayEndStep`, `IsGameClearStep`, `IsMoveStep`, `IsExitStep`)로 노출되며, 인덱스를 `TutorialConfigData` 상수와 비교해 매번 계산된다 (`SLAP_IDX=12`, `STANDING_IDX=16`, `ENTER_DREAM_IDX=19`, …) — 세터가 없다. `AdvanceIdx()` / `EnterDream()` / `ReturnToSubway()` / `EnterGameOver(kind)`가 상태를 바꾸고 `OnStepChanged`를 발행한다.

**TutorialSystem** (`Content/Tutorial/System/TutorialSystem.cs`)은 `DontDestroyOnLoad` 싱글톤으로 `ManagerInitializer` 목록에 있다 — `Player`(꿈 씬)의 static 이벤트 `OnNearExit` / `OnDreamExit`를 구독해야 해서 꿈 씬 로드 **전에** 존재해야 하기 때문 (`DreamManager`와 같은 이유). 상태는 갖지 않고 `Init()`에서 `TutorialData`를 캐싱만 한다. `EnterDreamPhase()` / `EnterGameOverPhase(kind)`가 진입을 조정하며, `UI_TutorialPopup` 참조(`TutorialPopup` 프로퍼티)도 여기 둔다 — 뷰 참조는 데이터가 아니므로 `TutorialData`에 두지 않는다.

> **대기 래치(await-action latch).** 예전 `TutorialManager`에서는 `SetTutorialTrigger()`의 유일한 호출부가 `UI_TutorialPopup.Update()`였고, 팝업이 플레이어 조작을 기다리며 스스로 비활성화되면 그 프레임에서 계산된 값이 그대로 얼어붙었다 — `HandleNearExit()`은 그 얼어붙은 값에 의존했다. `TutorialData`는 이를 명시적으로 재현한다: 팝업이 꺼질 때 `BeginAwaitAction()`이 게이트 인덱스를 그 순간 값으로 고정하고, 팝업의 `OnEnable()`에서 부르는 `EndAwaitAction()`이 풀어준다. 8개 단계 프로퍼티는 이 고정된 게이트(`GateIdx`/`GatePhase`)를 읽는다. 힌트 2개(`IsTirednessHintStep`, `IsTransferHintStep`)는 원본과 동일하게 예외다 — **라이브** `Phase`/`SubwayIdx`를 그대로 읽으며 래치를 거치지 않는다.

### SoundManager

Separate `AudioSource` components for BGM and SFX. Clips are cached by name and loaded from `Resources/Sounds/BGM/` and `Resources/Sounds/SFX/`. Volume and mute state are synced with `SaveManager` (PlayerPrefs keys: `BGM_VOLUME`, `SFX_VOLUME`, `BGM_MUTE`, `SFX_MUTE`). Use the typed helper methods (`PlayBGM()`, `SlapSFX()`, `JumpSFX()`, etc.) rather than raw Play calls.

### SaveManager

Thin wrapper around Unity `PlayerPrefs`. PlayerPrefs keys: `MaxClearStage`, `BGM_VOLUME`, `SFX_VOLUME`, `BGM_MUTE`, `SFX_MUTE`. Use typed methods (`LoadMaxClearDay()`, `SaveMaxClearDay()`, `LoadBgmVolume()`, etc.) rather than direct PlayerPrefs calls.

## Key Data Structures

| Type | Location | Purpose |
|------|----------|---------|
| `SubwayData` | `Content/Subway/Data/SubwayData.cs` | Owned by `GameDataManager.Subway`. Lines + progress + transfer + slap/standing state, and all subway events |
| `SubwayConfigData` | `Content/Subway/Data/SubwayConfigData.cs` | 지하철 상수 (흐름 속도, 뺨 쿨타임 등) |
| `LineData` | `Content/Subway/Data/LineData.cs` | List of `StationData`, `transferIdx`, `hasDestination` |
| `StationData` | `Content/Subway/Data/StationData.cs` | `stationType` (Normal/Transfer/Destination), `travelTime`, `stopTime` |
| `TirednessData` | `Content/Tiredness/Data/TirednessData.cs` | Owned by `GameDataManager.Tiredness`. `CurrentTiredness` / `MaxTiredness` / `IsPaused` |
| `TirednessConfigData` | `Content/Tiredness/Data/TirednessConfigData.cs` | 피로도 상수 (초기/최대값, 꿈 회복 기준, `STANDING_TIREDNESS`) |
| `TutorialData` | `Content/Tutorial/Data/TutorialData.cs` | Owned by `GameDataManager.Tutorial`. `Phase` / `SubwayIdx`/`DreamIdx`/`GameOverIdx` + 파생 `is*Step` 프로퍼티 |
| `TutorialConfigData` | `Content/Tutorial/Data/TutorialConfigData.cs` | 튜토리얼 트리거 인덱스 상수 (`SLAP_IDX`, `EXIT_IDX`, `DARK_GAMEOVER_IDX`, `TIREDNESS_HINT_IDX`, `TRANSFER_HINT_IDX` 등) |
| `TimerData` | `Content/Timer/Data/TimerData.cs` | Owned by `GameDataManager.Timer`. `PlayTime` / `IsPaused` (전역 일시정지) |
| `DreamData` | `Content/Dream/Data/DreamData.cs` | Owned by `GameDataManager.Dream`. `IsInDream` / `IsGameOverInDream` |
| `GameData` | `Content/GameData.cs` | Owned by `GameDataManager.Game`. `GameMode` / `CurrentDay` / `MaxClearDay` |
| `DialogLine` | ScriptManager | `Text` + `Emotion` fields for per-day story dialogue |

> **No static game state.** Earlier versions exposed values as `static` properties for convenient cross-system reads. That is gone — read through `GameDataManager.Instance.Subway` / `.Tiredness` instead, and subscribe to the data's **instance** events.

## Core Event Flow

**Dream entry**: Tiredness hits 100 → `TirednessData.OnTiredMaxed` → `SubwaySceneDirector.MoveToDreamScene()` (피로도 재계산 + `GoToDream()`). `SubwayPlayer.OnFellAsleep`(바로 잠들기)과 `OnSkipped`(입석 후 스킵)도 동일한 단일 경로를 통하며, `_isEnteringDream`이 세 경로의 중복 발화를 막는다. DreamScene 로드 후 `DreamSceneManager.Start()`가 `Subway.SetFlowSpeed(true)`로 흐름을 3–4배로 올린다 — 그동안 `SubwayFlowSystem`은 계속 틱한다.

**Dream exit**: Player touches `ExitDoor` → `WhitePanelSpawn` fades → SubwayScene reloads → `SubwaySceneDirector.Start()`가 `Subway.SetFlowSpeed(false)`로 복구. `StationSystem.Init()`은 `HasLines`가 true라 노선을 재생성하지 않으므로 진행 상태가 이어진다.

**Game over (subway)**: `SubwayData.SetGameOver()` → `OnSubwayGameOver` → `SubwaySceneDirector`가 `UI_GameOverPopup2` 표시. ⚠️ 현재 `SetGameOver()` 호출부가 없어 이 경로는 끊겨 있다 (위 Subway Rules 참고).

## Scripts Directory Map

```
Assets/Scripts/
├── Core/
│   ├── Managers/            # GameManager, GameDataManager, UIManager, SoundManager,
│   │   │                    # SaveManager, SceneTransitionManager
│   │   └── Base/            # SingletonManagers<T>, IManager
│   └── Interface/
├── Content/
│   ├── GameData.cs                # 전역 게임 데이터 (모드 + Day + 최고 기록)
│   ├── Subway/
│   │   ├── SubwaySceneDirector.cs # 씬 부트스트랩 + 진행 조정
│   │   ├── Data/                  # SubwayData, LineData, StationData, SubwayConfigData
│   │   └── System/                # SubwayFlowSystem(DontDestroyOnLoad), StationSystem(씬)
│   ├── Tiredness/
│   │   ├── Data/                  # TirednessData, TirednessConfigData
│   │   └── System/                # TirednessSystem
│   ├── Timer/
│   │   ├── Data/                  # TimerData (플레이 타임 + 전역 일시정지)
│   │   └── System/                # TimerSystem(DontDestroyOnLoad)
│   ├── Player/                    # SubwayPlayer (UI 반응 행동 + 애니메이션), AnimationEventHanlder
│   ├── Dream/
│   │   ├── Data/                  # DreamData
│   │   └── (DreamManager, DreamSceneManager, FogOrigin, map spawners)
│   ├── Map/                       # Background/parallax scrollers
│   ├── UI/
│   │   ├── Base/                  # UI_Base, UI_EventHandler
│   │   ├── Scene/                 # UI_Scene, UI_SubwayScene, UI_DreamScene, UI_MainScene, ...
│   │   └── Popup/                 # UI_Popup + all popup implementations
│   ├── Dialogue/                  # ScriptManager (per-day story text)
│   └── Tutorial/
│       ├── Data/                  # TutorialData (진행 상태), TutorialConfigData (트리거 인덱스 상수)
│       └── System/                # TutorialSystem(DontDestroyOnLoad)
└── Utils/                         # ManagerInitializer, Util, Define, SceneName
```

## Known Gaps

이미 파악됐지만 아직 고치지 않은 것들 — 관련 코드를 건드릴 때 참고할 것.

- **지하철 게임오버 경로 단절** — `SubwayData.SetGameOver()` 호출부가 없다. 환승 실패 감지가 `SubwayRuleManager` 삭제와 함께 사라졌다.
- **`TirednessSystem.SetTirednessOnDreamEnter()`의 도메인 결합** — 피로도 시스템이 `GameDataManager.Instance.Subway.CurrentLineTime`을 직접 읽는다. `TirednessData.ApplyDreamEnterRecovery(awakeTime)`는 파라미터로 받도록 잘 설계돼 있으므로, 호출부가 값을 넘기는 형태가 맞다.
- **`UI_SubwayScene.HideTirednessUI()`** — private이고 호출부가 없다.
- **`SubwayPlayer.TryTransfer()`** — 호출부가 없다. 애니메이터에 `isTransfer` 파라미터와 `PlayerTransferStanding` 상태가 실재하므로 환승 연출을 붙일 자리는 남아 있다.
