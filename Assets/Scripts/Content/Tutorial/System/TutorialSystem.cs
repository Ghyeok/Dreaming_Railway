using UnityEngine;

/// <summary>
/// 튜토리얼 진행 조정. 상태는 갖지 않고 TutorialData를 구동한다.
/// Player의 static 이벤트를 구독해야 하므로 꿈 씬 로드 전에 존재해야 한다
/// (그래서 ManagerInitializer 목록에 들어간다).
/// </summary>
public class TutorialSystem : SingletonManagers<TutorialSystem>, IManager
{
    private TutorialData _data;

    /// <summary>뷰 참조 — 데이터가 아니므로 TutorialData가 아니라 여기 둔다.</summary>
    public UI_TutorialPopup TutorialPopup { get; set; }

    public void Init()
    {
        _data = GameDataManager.Instance.Tutorial;

        // Init()이 두 번 호출돼도 중복 구독되지 않도록
        _data.OnFlowTimeChanged -= HandleFlowTimeChanged;
        _data.OnFlowTimeChanged += HandleFlowTimeChanged;
    }

    /// <summary>
    /// 튜토리얼의 흐름 시간 게이트를 전역 타이머에 반영한다.
    /// 예전에는 UI_TutorialPopup이 구독했는데, 팝업은 조작 대기 중 비활성화되고
    /// 씬 전환에 파괴되므로 꿈 탈출 시점의 SetFlowTime(true)를 놓쳤다.
    /// TutorialSystem은 DontDestroyOnLoad라 이벤트를 놓치지 않는다.
    /// </summary>
    private void HandleFlowTimeChanged(bool startFlowTime)
    {
        // 모드 가드 필수 — 없으면 튜토리얼 후 노말 모드 시작 시
        // TutorialData.Reset()의 SetFlowTime(false)가 노말 모드 타이머를 멈춘다.
        if (GameDataManager.Instance.Game.GameMode != GameMode.Tutorial) return;

        if (startFlowTime) GameDataManager.Instance.Timer.Resume();
        else GameDataManager.Instance.Timer.Pause();
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

    /// <summary>
    /// 지하철 씬 진입 — 튜토리얼 모드면 팝업을 띄운다. EnterDreamPhase()의 지하철 짝.
    /// </summary>
    public void EnterSubwayPhase()
    {
        if (GameDataManager.Instance.Game.GameMode != GameMode.Tutorial) return;

        _data ??= GameDataManager.Instance.Tutorial;

        if (_data.SubwayIdx < TutorialConfigData.GAME_CLEAR_IDX)
            TutorialPopup = UIManager.Instance.ShowPopupUI<UI_TutorialPopup>("UI_TutorialPopup");
    }

    /// <summary>꿈 씬 진입 — 예전 DreamManager가 필드 4개를 직접 대입하던 자리.</summary>
    public void EnterDreamPhase()
    {
        if (GameDataManager.Instance.Game.GameMode != GameMode.Tutorial) return;

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
