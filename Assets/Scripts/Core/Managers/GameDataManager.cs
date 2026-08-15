using UnityEngine;

public class GameDataManager : SingletonManagers<GameDataManager>
{
    public GameData Game => _game;
    public TirednessData Tiredness => _tiredness;
    public SubwayData Subway => _subway;
    public TimerData Timer => _timer;
    public DreamData Dream => _dream;

    // 인스펙터에서 진행 상태를 보기 위해 필드로 보유 (SubwayLines는 직렬화하지 않음)
    [SerializeField] private GameData _game = new();
    [SerializeField] private TirednessData _tiredness = new();
    [SerializeField] private SubwayData _subway = new();
    [SerializeField] private TimerData _timer = new();
    [SerializeField] private DreamData _dream = new();
}
