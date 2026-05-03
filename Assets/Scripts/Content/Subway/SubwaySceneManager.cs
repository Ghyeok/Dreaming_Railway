using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SubwaySceneManager : MonoBehaviour
{
    [SerializeField] private TirednessManager tirednessManager;
    [SerializeField] private StationManager stationManager;
    [SerializeField] private SubwayPlayerContext subwayPlayerContext;

    private void Start()
    {
        // ㅡㅡㅡㅡ 의존성 주입 ㅡㅡㅡㅡ
        stationManager.Init();
        tirednessManager.Init();
        subwayPlayerContext.Init(tirednessManager);

        // ㅡㅡㅡㅡ 씬 전환, 팝업 이벤트 구독 ㅡㅡㅡㅡ
        TirednessManager.OnTiredMaxed += MoveToDreamScene;
        subwayPlayerContext.OnStateChanged += MoveToDreamScene;
        SubwayFlowManager.Instance.OnDayCleared += OnDayCleared;

        // ㅡㅡㅡㅡ 그 외 공통 메서드 ㅡㅡㅡㅡ
        SoundManager.Instance.SubwayBGM();
    }

    // 지하철 -> 꿈 속 으로 전환될 때 실행되야 하는 모든 메서드들
    private void MoveToDreamScene()
    {
        DreamManager.Instance.SetDreamData(subwayPlayerContext.Rule.SlapNum);
        tirednessManager.SetTirednessAfterDream();
        SubwayFlowManager.Instance.SetFlowSpeed(true);
        SceneTransitionManager.Instance.GoToDream();
    }
    private void MoveToDreamScene(PlayerState state)
    {
        if (state == PlayerState.DEEPSLEEP)
            MoveToDreamScene();
    }

    private void OnDayCleared()
    {
        UIManager.Instance.ShowPopupUI<UI_GameClearPopup>("UI_GameClearPopup");
    }

    private void OnDestroy()
    {
        TirednessManager.OnTiredMaxed -= MoveToDreamScene;
        subwayPlayerContext.OnStateChanged -= MoveToDreamScene;
        SubwayFlowManager.Instance.OnDayCleared -= OnDayCleared;
    }
}