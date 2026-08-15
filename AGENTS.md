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
GameManager → SceneTransitionManager → SoundManager → UIManager → TimerSystem → DreamManager → SubwayFlowSystem
```

Access pattern everywhere: `GameManager.Instance`, `UIManager.Instance`, etc.

`GameDataManager` is deliberately **not** in that list — it does not implement `IManager` and is created lazily on first access (`SubwayFlowSystem.Init()` pulls it at `BeforeSceneLoad`, so creation is still deterministic).

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

`TimerData` owns `PlayTime` (shown on the GameOver/GameClear results screens) and the global pause flag `IsPaused`, mutated via `Pause()` / `Resume()`. `TimerSystem` is a `DontDestroyOnLoad` singleton that only calls `_data.Tick(Time.deltaTime)` — time must keep running across scene transitions, same as `SubwayFlowSystem`.

Per-frame gameplay systems (`SubwayFlowSystem`, `TirednessSystem`, `SubwayPlayerContext`) each run their own `Update()` and guard it with `if (_timer == null || _timer.IsPaused) return;`, where `_timer` is cached from `GameDataManager.Instance.Timer` in `Init()`. There is no central tick-dispatch interface. `Pause()` is used specifically where a popup (GameOver/GameClear) needs gameplay frozen without touching `Time.timeScale` (which would also freeze the popup's own fade-in). Full freezes that should also stop animations/coroutines use `GameManager.StopGame()/ResumeGame()` (`Time.timeScale = 0`) instead.

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

_station.Init();   // 노선이 없으면 생성 → _subway.BeginRun()
_tiredness.Init();
subwayPlayerContext.Init();

// 꿈속 진입 이벤트를 단일 지점에서 처리 (_isEnteringDream으로 중복 진입 차단)
_tirednessData.OnTiredMaxed += MoveToDreamScene;
subwayPlayerContext.OnStateChanged += MoveToDreamScene; // DEEPSLEEP 상태 필터
subwayPlayerContext.OnSkipped += MoveToDreamScene;      // 입석 후 스킵

// 팝업 표시도 Director가 담당 (데이터는 이벤트만 발행)
_subwayData.OnDayCleared += OnDayCleared;
_subwayData.OnSubwayGameOver += OnSubwayGameOver;
```

`MoveToDreamScene()`은 진입 전처리(피로도 조정, 흐름 속도 변경)와 `SceneTransitionManager.Instance.GoToDream()` 호출을 순서대로 담당한다. `Start()`의 `SetFlowSpeed(false)`가 꿈에서 걸린 3–4배속을 되돌리는 **유일한 지점**이므로 제거하면 안 된다.

> ⚠️ **프리팹 안의 컴포넌트에 씬 오브젝트 참조를 `[SerializeField]`로 물리지 말 것.** `SubwayPlayerContext`는 `UI_SubwayScene.prefab` 소속이라, 씬 오브젝트를 가리키는 필드는 프리팹 애셋에 저장될 수 없고 프리팹 인스턴스 override로만 남는다 — Revert 한 번이나 다른 씬 배치에서 조용히 null이 된다. 그런 의존은 만들지 말고 데이터를 직접 쓰도록 설계할 것.

### Tiredness (TirednessData + TirednessSystem)

`TirednessData.Tick()` accumulates `CurrentTiredness += deltaTime` and fires `OnTiredChange` every tick, `OnTiredMaxed` at 100. On dream entry, `ApplyDreamEnterRecovery(awakeTime)` halves tiredness if awake time ≤ 100 s, otherwise scales it by 2/3.

`IsPaused`도 **데이터가 소유한다.** 입석 시 `SetForced(STANDING_TIREDNESS)`가 값을 99.9로 고정하면서 증가를 멈추고 (`Tick()`이 `IsPaused`에서 조기 return), `TirednessSystem.Init()`의 `Resume()`이 지하철 씬 재진입 때 해제한다. 99.9인 이유는 100 미만이어야 `OnTiredMaxed`로 인한 자동 꿈 진입이 걸리지 않기 때문.

`TirednessSystem`은 틱 구동 전용이다 — 상태도, 값 변경 래퍼도 갖지 않는다. **값을 바꾸는 쪽은 시스템을 거치지 말고 `TirednessData`를 직접 호출한다** (`SubwayPlayerContext`가 `_tiredness.Decrease()` / `_tiredness.SetForced()`를 직접 부르는 식). 시스템 참조를 주입하면 위의 프리팹 제약에 걸린다.

### Subway Flow (SubwayData + SubwayFlowSystem)

`SubwayData.Tick()` is the state machine. It alternates between travelling (`IsSubwayStopping = false`) and stopped at a station (`IsSubwayStopping = true`), driven by `TimeToNextState` counting down at `FlowSpeed` (1× normally, 3.1–4.1× while in the dream scene). `Tick()` no-ops until `HasLines` is true, so it is safe for it to run before `StationSystem` has generated the lines.

`SubwayFlowSystem` is a **`DontDestroyOnLoad` singleton, not a scene object** — the subway must keep ticking while the player is in DreamScene, which is the core mechanic. It holds no state; it only calls `_data.Tick()` and forwards `OnDayCleared` to `GameManager.UpdateMaxClearDay()` (that must happen even when the day is cleared mid-dream, where `SubwaySceneDirector` doesn't exist).

`StationSystem` generates the line data once per run (travel time 10–15 s, stop time 6–8 s, 20 stations per line) and hands it over with `_subway.BeginRun(lines, maxTransferCount)`. It skips generation entirely when `_subway.HasLines` is already true, which is how state survives the DreamScene round trip. Transfer count: `day + 1` in normal mode, 999 in infinite mode.

> `IsRunFinished` is set when the last transfer completes. It is required: without it `Tick()`'s `while` loop would index past the end of `SubwayLines` forever and freeze the editor.

### Subway Rules (in SubwayData)

The former `SubwayRuleManager` is gone; its rules now live as data + `SubwayPlayerContext` actions:
- **Slap**: `SubwayPlayerContext.TrySlap()` → `SubwayData.StartSlapCooldown()`, reduces tiredness by `TiredDecreaseBySlap` (3 normal / 4 infinite) with a 5-second cooldown ticked by `TickSlapCooldown()`.
- **Standing**: `TryStand()` forces tiredness to 99.9; `TrySkip()` calls `SubwayData.ForceTransferByStanding()`. After standing, the player cannot stand again until 2 more lines end (`AddStandingCount()` subscribed to `OnLineEnded`).
- **Game over**: `SubwayData.SetGameOver()` fires `OnSubwayGameOver`; `SubwaySceneDirector` shows the popup. ⚠️ Nothing currently calls `SetGameOver()` — the missed-transfer detection was lost when `SubwayRuleManager` was deleted, so the subway game-over path is dead.

### Player State Machine (SubwayPlayerContext)

The state-class FSM was removed; state is now a plain `PlayerState` enum (`NONE / SLEEP / STANDING / DEEPSLEEP`) switched by `ChangeState()`, which fires `OnStateChanged`:

- **SLEEP** — default; tiredness accumulates
- **STANDING** — plays `isStanding` animation, triggers `StandingSFX()`, forces tiredness to 99.9
- **DEEPSLEEP** — `SubwaySceneDirector` filters this state to trigger the dream scene load

`SubwayPlayerContext.Data` is **the shared `GameDataManager.Instance.Subway` instance**, not a private copy — `Init()` calls `Data.ResetPlayerSession(isInfiniteMode)` to clear the per-run slap/standing values. `AnimationEventHandler` (`Assets/Scripts/Content/Player/AnimationEventHanlder.cs`) bridges Unity animation events to player callbacks.

### UI Framework

All UI inherits from `UI_Base` (`Assets/Scripts/Content/UI/Base/`). The binding system maps child GameObjects to enum-named entries:

```csharp
Bind<Button>(typeof(Buttons));      // binds by enum name matching GameObject name
Button btn = GetButton((int)Buttons.StartButton);
```

`UI_Popup` is stack-managed by `UIManager` (`ShowPopupUI<T>(name)` / `ClosePopupUI()`). `UI_Scene` is a single persistent overlay per scene (`ShowSceneUI<T>(name)`). All UI prefabs live under `Resources/Prefabs/UIs/`.

### Dream (DreamData + DreamManager)

꿈 상태는 `DreamData`(`GameDataManager.Dream`)가 소유한다 — `IsInDream`, `IsGameOverInDream`. `DreamManager`(`Assets/Scripts/Content/Dream/DreamManager.cs`)는 상태를 갖지 않고 진입만 조정한다: `SceneManager.sceneLoaded`를 구독해 꿈 씬 진입 시 `EnterDream()`을 호출하고, 튜토리얼 모드에서는 `TutorialManager`와 협조해 진행을 게이트한다.

> ⚠️ **`DreamManager`는 `ManagerInitializer` 목록에 반드시 있어야 한다.** 꿈 씬이 로드되기 *전에* 생성돼 `sceneLoaded`를 구독하고 있어야 하기 때문 — 빠지면 `IsInDream`이 영영 false로 남아 일시정지/설정/노선 팝업이 조용히 오작동한다.

`IsGameOverInDream`은 새 런 시작 시 `DreamData.Reset()`으로 해제된다. 호출 지점이 **두 곳**인 이유는 무한 모드가 `StartDay()`를 거치지 않기 때문이다: `GameManager.StartDay()`(튜토리얼+노말)와 `UI_MainScene.InfiniteModeOnClicked()`(무한). 리셋 창구가 통합되면 하나로 합쳐질 자리다.

뺨 횟수는 `DreamData`에 두지 않는다 — 맵 스포너(`MapXSpawn`, `MapYSpawn`)가 `GameDataManager.Instance.Subway.SlapNum`을 직접 읽는다. 예전에는 꿈 진입 시 복사본을 만들었는데, 원본과 조용히 어긋날 수 있어 제거했다.

`DreamSceneManager` is the dream-side counterpart to `SubwaySceneDirector` — it controls dream init order and calls `Subway.SetFlowSpeed(true)`. `FogOrigin` centralizes the fog/map/camera direction decision.

### Dialogue & Tutorial

**ScriptManager** (`Assets/Scripts/Content/Dialogue/ScriptManager.cs`): Loads per-day story text as `DialogLine` structs (Text + Emotion fields). Shows `UI_ScriptPopup` at day start and after clearing. Emotion sprites are loaded from `Resources/`.

**TutorialManager** (`Assets/Scripts/Content/Tutorial/TutorialManager.cs`): Drives multi-phase tutorial (Subway, Dream, GameOver phases) via progression indices (e.g., `slapIdx=12`, `standingIdx=16`, `enterDreamIdx=19`). Controls tutorial popup visibility and overlays. Uses additive scene loading for `TutorialScene` and `ScriptScene`.

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
| `TimerData` | `Content/Timer/Data/TimerData.cs` | Owned by `GameDataManager.Timer`. `PlayTime` / `IsPaused` (전역 일시정지) |
| `DreamData` | `Content/Dream/Data/DreamData.cs` | Owned by `GameDataManager.Dream`. `IsInDream` / `IsGameOverInDream` |
| `GameData` | `Content/GameData.cs` | Owned by `GameDataManager.Game`. `GameMode` / `CurrentDay` / `MaxClearDay` |
| `DialogLine` | ScriptManager | `Text` + `Emotion` fields for per-day story dialogue |

> **No static game state.** Earlier versions exposed values as `static` properties for convenient cross-system reads. That is gone — read through `GameDataManager.Instance.Subway` / `.Tiredness` instead, and subscribe to the data's **instance** events.

## Core Event Flow

**Dream entry**: Tiredness hits 100 → `TirednessData.OnTiredMaxed` → `SubwaySceneDirector.MoveToDreamScene()` (피로도 재계산 + `GoToDream()`). DeepSleep 상태 진입과 입석 스킵도 동일한 단일 경로를 통하며, `_isEnteringDream`이 세 경로의 중복 발화를 막는다. DreamScene 로드 후 `DreamSceneManager.Start()`가 `Subway.SetFlowSpeed(true)`로 흐름을 3–4배로 올린다 — 그동안 `SubwayFlowSystem`은 계속 틱한다.

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
│   ├── Player/                    # SubwayPlayerContext (enum 기반 상태), AnimationEventHanlder
│   ├── Dream/
│   │   ├── Data/                  # DreamData
│   │   └── (DreamManager, DreamSceneManager, FogOrigin, map spawners)
│   ├── Map/                       # Background/parallax scrollers
│   ├── UI/
│   │   ├── Base/                  # UI_Base, UI_EventHandler
│   │   ├── Scene/                 # UI_Scene, UI_SubwayScene, UI_DreamScene, UI_MainScene, ...
│   │   └── Popup/                 # UI_Popup + all popup implementations
│   ├── Dialogue/                  # ScriptManager (per-day story text)
│   └── Tutorial/                  # TutorialManager (multi-phase tutorial)
└── Utils/                         # ManagerInitializer, Util, Define, SceneName
```

## Known Gaps

이미 파악됐지만 아직 고치지 않은 것들 — 관련 코드를 건드릴 때 참고할 것.

- **지하철 게임오버 경로 단절** — `SubwayData.SetGameOver()` 호출부가 없다. 환승 실패 감지가 `SubwayRuleManager` 삭제와 함께 사라졌다.
- **Day 재시작 시 데이터 리셋 창구 부재** — `TirednessSystem.ResetTiredness()`는 정의만 있고 호출부가 없다. `GameDataManager`에 리셋 진입점이 없는 것이 근본 원인. `DreamData.Reset()`은 임시로 `GameManager.StartDay()` + `UI_MainScene.InfiniteModeOnClicked()` 두 곳에서 직접 호출하고 있는데, 통합 진입점(`GameDataManager.ResetForNewRun()` 등)이 생기면 그리로 흡수돼야 한다.
- **`TirednessSystem.SetTirednessOnDreamEnter()`의 도메인 결합** — 피로도 시스템이 `GameDataManager.Instance.Subway.CurrentLineTime`을 직접 읽는다. `TirednessData.ApplyDreamEnterRecovery(awakeTime)`는 파라미터로 받도록 잘 설계돼 있으므로, 호출부가 값을 넘기는 형태가 맞다.
- **`UI_SubwayScene.HideTirednessUI()`** — private이고 호출부가 없다.
