using System;
using UnityEngine;

public class GameManager : SingletonManagers<GameManager>, IManager
{
    [Header("Game State & Mode")]
    public GameMode GameMode { get; private set; } // 현재 게임 모드(노말, 무한)
    public bool IsGameStopped { get; private set; }

    [Header("Progress Data")]
    public int CurrentDay { get; private set; } = 1; // 현재 플레이 중인 Day
    public int MaxClearDay { get; private set; } = -1; // 최대로 클리어한 Day

    public static event Action<GameMode> OnGameModeChanged; // 게임 모드가 변할 때 Invoke

    public void Init()
    {
        MaxClearDay = SaveManager.Instance.LoadMaxClearDay();
    }

    private void ResetGameManager()
    {
        IsGameStopped = false;
    }

    public void ResetGame()
    {
        ResetGameManager();
        // 필요시 다른 매니저들 리셋 호출
    }

    public void StartDay(int day)
    {
        CurrentDay = day;
    }

    public void UpdateMaxClearDay()
    {
        if (CurrentDay > MaxClearDay)
        {
            MaxClearDay = CurrentDay;
            PlayerPrefs.SetInt("MaxClearStage", MaxClearDay);
            PlayerPrefs.Save();
        }
    }

    public void ChangeGameMode(GameMode newMode)
    {
        if (GameMode == newMode) return;

        GameMode = newMode;
        OnGameModeChanged?.Invoke(newMode);
    }

    public void StopGame()
    {
        Time.timeScale = 0f;
        IsGameStopped = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsGameStopped = false;
    }
}