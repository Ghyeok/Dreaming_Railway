
using System;
using UnityEngine;

public class GameData
{
    public GameMode GameMode { get; private set; } = GameMode.None;
    public int CurrentDay { get; private set; } = 1;
    public int MaxClearDay { get; private set; } = 0;

    public event Action OnGameStarted;
    public event Action<GameMode> OnGameModeChanged;

    /// <summary>
    /// Day를 선택하면 CurrendDay가 그에 맞는 day로 변경된다
    /// </summary>
    /// <param name="day"></param>
    public void StartDay(int day) { CurrentDay = day; }

    /// <summary>
    /// Day를 클리어하면 MaxClearDay가 클리어한 Day로 변경된다.
    /// </summary>
    public void UpdateMaxClearDay()
    {
        if (CurrentDay > MaxClearDay) {  MaxClearDay = CurrentDay; }
    }

    /// <summary>
    /// MaxClearDay를 게임 시작과 동시에 초기화 시켜줘야 한다.
    /// </summary>
    public void InitMaxClearDay(int max) { MaxClearDay = max; }

    /// <summary>
    /// 게임 모드를 변경하는 함수. 게임 모드를 선택하면 모드에 맞는 OnGameModeChanged 콜백이 실행된다.
    /// </summary>
    /// <param name="newMode"></param>
    public void ChangeGameMode(GameMode newMode)
    {
        if (GameMode == newMode) return;

        GameMode = newMode;
        OnGameModeChanged?.Invoke(newMode);
    }
}

