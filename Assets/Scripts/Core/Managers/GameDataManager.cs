using UnityEngine;

public class GameDataManager : SingletonManagers<GameDataManager>
{
    public GameData Game { get; private set; }
    public TirednessData Tiredness { get; private set; }
    public SubwayData Subway => _subway;

    // 인스펙터에서 진행 상태를 보기 위해 필드로 보유 (SubwayLines는 직렬화하지 않음)
    [SerializeField] private GameData _game = new();
    [SerializeField] private TirednessData _tiredness = new();
    [SerializeField] private SubwayData _subway = new();
}
