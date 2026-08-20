using UnityEngine;

/// <summary>
/// 지하철 씬의 진행을 총괄한다.
/// Start()에서 씬 부트스트랩(초기화 순서 제어 + 1회성 로직)을 수행하고,
/// 이후 씬이 유지되는 동안 꿈씬 진입과 팝업 표시를 단일 지점에서 조정한다.
/// </summary>
[DefaultExecutionOrder(-100)]
public class SubwaySceneDirector : MonoBehaviour
{
    [SerializeField] private TirednessSystem _tiredness;
    [SerializeField] private StationSystem _station;
    [SerializeField] private SubwayPlayer _subwayPlayer;

    private SubwayData _subwayData;
    private TirednessData _tirednessData;

    private bool _isEnteringDream = false;

    private void Start()
    {
        // ㅡㅡㅡㅡ 게임 데이터 참조 캐싱 ㅡㅡㅡㅡ
        _subwayData = GameDataManager.Instance.Subway;
        _tirednessData = GameDataManager.Instance.Tiredness;

        // ㅡㅡㅡㅡ 초기화 순서 제어 및 의존성 주입 ㅡㅡㅡㅡ
        // 씬을 넘어 살아있는 전역 시스템 — 지하철 씬에 들어올 때마다 데이터를 다시 물린다
        TimerSystem.Instance.Init();
        SubwayFlowSystem.Instance.Init();

        _station.Init();
        _tiredness.Init();
        _subwayPlayer.Init();

        // ㅡㅡㅡㅡ 그 외 일회성 메서드 ㅡㅡㅡㅡ
        // 지하철 Scene이 로드되고 한번 실행되야 하는 메서드들
        SoundManager.Instance.SubwayBGM();
        _subwayData.SetFlowSpeed(false);

        // ㅡㅡㅡㅡ 씬 전환 이벤트 구독 ㅡㅡㅡㅡ
        _tirednessData.OnTiredMaxed += MoveToDreamScene; // 피로도 100
        _subwayPlayer.OnFellAsleep += MoveToDreamScene; // 바로 잠들기
        _subwayPlayer.OnSkipped += MoveToDreamScene; // 입석 후 스킵
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
        _subwayPlayer.OnFellAsleep -= MoveToDreamScene;
        _subwayPlayer.OnSkipped -= MoveToDreamScene;

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
