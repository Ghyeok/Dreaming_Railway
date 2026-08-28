using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SubwaySceneInitializer : MonoBehaviour
{
    [SerializeField] private TirednessSystem _tiredness;
    [SerializeField] private StationSystem _station;
    [SerializeField] private SubwayPlayerContext subwayPlayerContext;

    private SubwayData _subwayData;
    private TirednessData _tirednessData;
    private TimerData _timerData;

    private bool _isEnteringDream = false;

    private void Start()
    {
        // ㅡㅡㅡㅡ 게임 데이터 참조 캐싱 ㅡㅡㅡㅡ
        _subwayData = GameDataManager.Instance.Subway;
        _tirednessData = GameDataManager.Instance.Tiredness;
        _timerData = GameDataManager.Instance.Timer;

        // ㅡㅡㅡㅡ 초기화 순서 제어 및 의존성 주입 ㅡㅡㅡㅡ
        _station.Init();
        _tiredness.Init();
        subwayPlayerContext.Init(_tiredness);

        // ㅡㅡㅡㅡ 그 외 일회성 메서드 ㅡㅡㅡㅡ
        // 지하철 Scene이 로드되고 한번 실행되야 하는 메서드들
        SoundManager.Instance.SubwayBGM();
        _subwayData.SetFlowSpeed(false);

        // 게임오버 팝업이 멈춰둔 흐름을 재개한다. Retry는 런을 이어서 진행하므로 PlayTime은 유지한다.
        _timerData.Resume();

        // ㅡㅡㅡㅡ 씬 전환 이벤트 구독 ㅡㅡㅡㅡ
        _tirednessData.OnTiredMaxed += MoveToDreamScene; // 피로도 100
        subwayPlayerContext.OnStateChanged += MoveToDreamScene; // DEEPSLEEP 상태면
        subwayPlayerContext.OnSkipped += MoveToDreamScene; // 입석 후 스킵
        _subwayData.OnDayCleared += OnDayCleared;
        _subwayData.OnSubwayGameOver += OnSubwayGameOver;
    }

    // 지하철 -> 꿈 속 으로 전환될 때 실행되야 하는 모든 메서드들
    private void MoveToDreamScene()
    {
        if (_isEnteringDream) return;
        _isEnteringDream = true;

        _tiredness.SetTirednessOnDreamEnter();
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

    private void OnSubwayGameOver()
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_GameOverPopup2");
    }

    private void OnDestroy()
    {
        subwayPlayerContext.OnStateChanged -= MoveToDreamScene;
        subwayPlayerContext.OnSkipped -= MoveToDreamScene;

        // 순수 C# 데이터 객체이므로 종료 중에도 null이 되지 않는다 (싱글톤 재접근 회피)
        if (_tiredness != null)
            _tirednessData.OnTiredMaxed -= MoveToDreamScene;

        if (_subwayData != null)
        {
            _subwayData.OnDayCleared -= OnDayCleared;
            _subwayData.OnSubwayGameOver -= OnSubwayGameOver;
        }
    }
}
