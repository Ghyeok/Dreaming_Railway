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
