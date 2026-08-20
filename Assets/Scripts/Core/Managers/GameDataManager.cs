using UnityEngine;

public class GameDataManager : SingletonManagers<GameDataManager>, IManager
{
    public void Init() { }

    [SerializeField] private GameData _game = new();
    [SerializeField] private TirednessData _tiredness = new();
    [SerializeField] private SubwayData _subway = new();
    [SerializeField] private TimerData _timer = new();
    [SerializeField] private DreamData _dream = new();
    [SerializeField] private TutorialData _tutorial = new();

    public GameData Game => _game;
    public TirednessData Tiredness => _tiredness;
    public SubwayData Subway => _subway;
    public TimerData Timer => _timer;
    public DreamData Dream => _dream;
    public TutorialData Tutorial => _tutorial;

    /// <summary>
    /// 새로운 게임을 위한 인게임 데이터 전체 초기화
    /// </summary>
    public void ResetForNewRun()
    {
        _tiredness.Reset();
        // 순서 주의: _tutorial.Reset()의 SetFlowTime(false)가 TutorialSystem을 통해
        // Timer를 멈추므로, _timer.Reset()이 먼저 와야 그 일시정지가 살아남는다.
        _timer.Reset();
        _dream.Reset();
        _subway.Reset();
        _tutorial.Reset();
    }
}
