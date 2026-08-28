using UnityEngine;

public class SubwayFlowSystem : SingletonManagers<SubwayFlowSystem>, IManager
{
    private SubwayData _subway;
    private GameData _game;

    public void Init()
    {
        _subway = GameDataManager.Instance.Subway;

        _subway.OnDayCleared -= HandleDayCleared;
        _subway.OnDayCleared += HandleDayCleared;
    }

    private void Update()
    {
        if (_subway == null) return;
        if (TimerManager.Instance == null || TimerManager.Instance.IsPaused) return;
        if (_game.IsGameStopped) return;

        _subway.Tick(Time.deltaTime);
    }

    private void HandleDayCleared() => _game.UpdateMaxClearDay();
}
