using UnityEngine;

public class GameDataManager : SingletonManagers<GameDataManager>, IManager
{
    public void Init() { }

    public GameData Game => _game;
    public TirednessData Tiredness => _tiredness;
    public SubwayData Subway => _subway;
    public TimerData Timer => _timer;

    // 인스펙터에서 진행 상태를 보기 위해 필드로 보유 (SubwayLines는 직렬화하지 않음)
    [SerializeField] private GameData _game = new();
    [SerializeField] private TirednessData _tiredness = new();
    [SerializeField] private SubwayData _subway = new();
    [SerializeField] private TimerData _timer = new();

    /// <summary>
    /// 새 런을 시작할 때 런 단위 데이터를 초기화한다. 각 데이터의 Reset()을 개별 호출하지 말고 이곳으로 모은다.
    /// </summary>
    public void ResetForNewRun()
    {
        _timer.Reset();
    }
}
