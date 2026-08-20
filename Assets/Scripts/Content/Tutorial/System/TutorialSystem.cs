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

        if (_data.DreamIdx < TutorialConfigData.EXIT_IDX)
            TutorialPopup = UIManager.Instance.ShowPopupUI<UI_TutorialPopup>("UI_TutorialPopup");
    }

    /// <summary>게임오버 — 예전 UI_GameOverPopup이 필드 4개를 직접 대입하던 자리.</summary>
    public void EnterGameOverPhase(GameOverKind kind)
    {
        _data ??= GameDataManager.Instance.Tutorial;

        _data.EnterGameOver(kind);

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
