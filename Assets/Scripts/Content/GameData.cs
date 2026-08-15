using System;
using UnityEngine;

/// <summary>
/// 전역 게임 진행 데이터. 씬과 무관하게 GameDataManager가 소유한다.
/// 영속화(SaveManager)는 여기서 하지 않는다 — 데이터 클래스는 매니저를 참조하지 않는다.
/// </summary>
[System.Serializable]
public class GameData
{
    [field: SerializeField] public GameMode GameMode { get; private set; } = GameMode.None; // 현재 게임 모드(노말, 무한)

    [field: SerializeField] public int CurrentDay { get; private set; } = 1; // 현재 플레이 중인 Day

    [field: SerializeField] public int MaxClearDay { get; private set; } = -1; // 최대로 클리어한 Day

    public event Action<GameMode> OnGameModeChange; // 게임 모드가 변할 때 Invoke

    public void SetGameMode(GameMode newMode)
    {
        if (GameMode == newMode) return;

        GameMode = newMode;
        OnGameModeChange?.Invoke(newMode);
    }

    public void SetCurrentDay(int day) => CurrentDay = day;

    /// <summary>
    /// 저장된 최고 기록을 불러올 때 사용
    /// </summary>
    public void SetMaxClearDay(int day) => MaxClearDay = day;

    /// <summary>
    /// 현재 Day가 최고 기록을 갱신하면 갱신하고 true를 반환한다.
    /// 호출부는 이 반환값으로 저장 여부를 결정한다.
    /// </summary>
    public bool TryUpdateMaxClearDay()
    {
        if (CurrentDay <= MaxClearDay) return false;

        MaxClearDay = CurrentDay;
        return true;
    }
}
