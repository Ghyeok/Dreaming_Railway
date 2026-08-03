using UnityEngine;

public class SubwayFlowSystem : SingletonManagers<SubwayFlowSystem>, IManager
{
    private SubwayData _data;

    public void Init()
    {
        _data = GameDataManager.Instance.Subway;

        // Init이 두 번 호출돼도 중복 구독되지 않도록
        _data.OnDayCleared -= HandleDayCleared;
        _data.OnDayCleared += HandleDayCleared;
    }

    private void Update()
    {
        if (_data == null) return;
        if (TimerManager.Instance == null || TimerManager.Instance.IsPaused) return;
        if (GameManager.Instance.IsGameStopped) return;

        _data.Tick(Time.deltaTime);
    }

    // Day 클리어는 꿈 씬 안에서도 발생할 수 있다.
    // SubwaySceneManager는 그때 존재하지 않으므로 전역 싱글톤인 이곳에서 저장을 처리한다.
    private void HandleDayCleared() => GameManager.Instance.UpdateMaxClearDay();
}
