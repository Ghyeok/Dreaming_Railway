using UnityEngine;

public class SubwayFlowSystem : SingletonManagers<SubwayFlowSystem>, IManager
{
    private SubwayData _subway;
    private GameData _game;
    private TimerData _timer;

    public void Init()
    {
        _subway = GameDataManager.Instance.Subway;
        _game = GameDataManager.Instance.Game;
        _timer = GameDataManager.Instance.Timer;

        _subway.OnDayCleared -= HandleDayCleared;
        _subway.OnDayCleared += HandleDayCleared;
    }

    private void Update()
    {
        if (_subway == null) return;
        if (_timer == null || _timer.IsPaused) return;

        _subway.Tick(Time.deltaTime);
    }

    private void HandleDayCleared()
    {
        _game.UpdateMaxClearDay();
        SaveManager.Instance.SaveMaxClearDay(_game.MaxClearDay);
    }
}
