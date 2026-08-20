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
    // 지금은 아무도 읽지 않는다 — GameOverKind.Passed 경로가 아직 도달 불가능이라서다.
    // 그 경로가 복구되면 분기에 쓸 자리이므로 남겨 둔다 (죽은 상태 아님).
    [field: SerializeField] public GameOverKind OverKind { get; private set; } = GameOverKind.Dark;

    /// <summary>
    /// 현재 단계를 이미 소비했는가를 기록한다. 인덱스가 바뀌면 자동으로 무효화된다.
    /// 예전에 외부에서 `isMoveTutorial = false`로 끄던 래치를 대체한다.
    /// </summary>
    private int _consumedIdx = -1;

    // AdvanceIdx()가 증가 직전 값을 남겨 둔다. BeginAwaitAction()이 이걸 사용한다.
    private int _prevIdx = -1;
    private TutorialPhase _prevPhase = TutorialPhase.Subway;

    // 팝업이 꺼져 플레이어 조작을 기다리는 동안, 게이트가 읽을 단계를 붙잡아 둔다.
    // 예전 구조에서는 SetTutorialTrigger()의 유일한 호출부가 팝업의 Update였기 때문에,
    // 팝업이 꺼지면 플래그가 그 시점 값으로 얼어붙어 같은 역할을 했다.
    private int _awaitingIdx = -1;
    private TutorialPhase _awaitingPhase = TutorialPhase.Subway;

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

    /// <summary>플레이어 조작을 기다리는 중인가 (팝업이 꺼져 있는 구간)</summary>
    private bool IsAwaitingAction => _awaitingIdx >= 0;

    /// <summary>단계 게이트가 읽는 Phase — 대기 중이면 얼어붙은 값</summary>
    private TutorialPhase GatePhase => IsAwaitingAction ? _awaitingPhase : Phase;

    /// <summary>단계 게이트가 읽는 인덱스 — 대기 중이면 얼어붙은 값</summary>
    private int GateIdx => IsAwaitingAction ? _awaitingIdx : CurrentIdx;

    private bool IsConsumed => _consumedIdx == GateIdx;

    public bool IsSlapStep       => GatePhase == TutorialPhase.Subway && GateIdx == TutorialConfigData.SLAP_IDX && !IsConsumed;
    public bool IsStandingStep   => GatePhase == TutorialPhase.Subway && GateIdx == TutorialConfigData.STANDING_IDX && !IsConsumed;
    public bool IsSkipStep       => GatePhase == TutorialPhase.Subway && GateIdx == TutorialConfigData.SKIP_IDX && !IsConsumed;
    public bool IsEnterDreamStep => GatePhase == TutorialPhase.Subway && GateIdx == TutorialConfigData.ENTER_DREAM_IDX && !IsConsumed;
    public bool IsSubwayEndStep  => GatePhase == TutorialPhase.Subway && GateIdx == TutorialConfigData.SUBWAY_END_IDX && !IsConsumed;
    public bool IsGameClearStep  => GatePhase == TutorialPhase.Subway && GateIdx == TutorialConfigData.GAME_CLEAR_IDX && !IsConsumed;
    public bool IsMoveStep       => GatePhase == TutorialPhase.Dream  && GateIdx == TutorialConfigData.MOVE_IDX && !IsConsumed;
    public bool IsExitStep       => GatePhase == TutorialPhase.Dream  && GateIdx == TutorialConfigData.EXIT_IDX && !IsConsumed;

    // 힌트 2개는 라이브 값(Phase / SubwayIdx)을 그대로 읽는다.
    // 원본에서 subwayIdx == 8 / == 9는 SetTutorialTrigger()를 거치지 않고 Update()가 직접 읽어
    // 얼지 않았고, 팝업이 켜져 있을 때만 읽히므로 대기 구간과 무관하다.
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
        // 주의: 반드시 증가 '전에' 기록한다. BeginAwaitAction()이 이 값으로 게이트를 고정하므로
        // 증가 후에 캡처하면 래치가 한 칸 밀린 단계에 얼어붙는다.
        _prevPhase = Phase;
        _prevIdx = CurrentIdx;

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
        // 이전 phase에서 얼어붙은 게이트가 새 phase까지 살아남으면 안 되므로 대기 래치도 함께 푼다.
        EndAwaitAction();
        OnStepChanged?.Invoke();
    }

    public void ReturnToSubway()
    {
        Phase = TutorialPhase.Subway;
        _consumedIdx = -1;
        // 이전 phase에서 얼어붙은 게이트가 새 phase까지 살아남으면 안 되므로 대기 래치도 함께 푼다.
        EndAwaitAction();
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
        // 이전 phase에서 얼어붙은 게이트가 새 phase까지 살아남으면 안 되므로 대기 래치도 함께 푼다.
        EndAwaitAction();
        OnStepChanged?.Invoke();
    }

    public void SetFlowTime(bool value)
    {
        if (StartFlowTime == value) return;

        StartFlowTime = value;
        OnFlowTimeChanged?.Invoke(value);
    }

    /// <summary>현재 단계를 소비 처리해 같은 인덱스에서 재발화하지 않게 한다.</summary>
    public void ConsumeCurrentStep() => _consumedIdx = GateIdx;

    /// <summary>
    /// 팝업이 꺼지며 플레이어 조작을 기다리기 시작한다.
    /// 게이트를 '지금 요구 중인 단계'(= AdvanceIdx 직전 값)에 고정한다.
    /// AdvanceIdx() 직후에만 호출해야 의미가 있다.
    /// </summary>
    public void BeginAwaitAction()
    {
        _awaitingPhase = _prevPhase;
        _awaitingIdx = _prevIdx;
    }

    /// <summary>
    /// 기다리던 조작이 끝나 팝업이 다시 보인다. 게이트를 라이브 값으로 되돌린다.
    /// </summary>
    public void EndAwaitAction() => _awaitingIdx = -1;

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
        _prevIdx = -1;
        _prevPhase = TutorialPhase.Subway;
        _awaitingIdx = -1;
        _awaitingPhase = TutorialPhase.Subway;

        SetFlowTime(false);
        OnStepChanged?.Invoke();
    }
}
