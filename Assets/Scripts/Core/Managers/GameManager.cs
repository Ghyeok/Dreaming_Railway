using System;
using UnityEngine;

[System.Serializable]
public class SubwayRunData
{
    public int CurrentLineIdx;      // 현재 호선 인덱스
    public float CurrentLineTime;   // 현재 호선에서 주행한 총 시간
    public int PassedStations;      // 지나친 역의 개수
    public bool IsMissedTransfer;   // 환승역을 놓쳤는지 여부

    public void ResetData()
    {
        CurrentLineIdx = 0;
        CurrentLineTime = 0f;
        PassedStations = 0;
        IsMissedTransfer = false;
    }

    public void TickSubwayTime(float deltaTime, float speed, float timePerStation, int transferStationIdx)
    {
        if (IsMissedTransfer) return;

        // 1. 시간 누적
        CurrentLineTime += deltaTime * speed;

        // 2. 현재 시간을 기준으로 몇 정거장을 왔는지 계산
        int calculatedStation = Mathf.FloorToInt(CurrentLineTime / timePerStation);

        // 3. 만약 역을 하나 더 지나쳤다면 갱신
        if (calculatedStation > PassedStations)
        {
            PassedStations = calculatedStation;

            // 4. 환승역을 지나쳤는지 검사!
            if (PassedStations > transferStationIdx)
            {
                IsMissedTransfer = true;
                Debug.Log("🚨 [글로벌 데이터] 환승역을 지나쳤습니다!");
            }
        }
    }
}

public class GameManager : SingletonManagers<GameManager>, IManager
{
    [Header("Game State & Mode")]
    public GameMode GameMode { get; private set; } // 현재 게임 모드(노말, 무한)
    public bool IsGameStopped { get; private set; }

    [Header("Progress Data")]
    public int CurrentDay { get; private set; } = 1; // 현재 플레이 중인 Day
    public int MaxClearDay { get; private set; } = -1; // 최대로 클리어한 Day

    public SubwayRunData RunData { get; private set; } = new SubwayRunData();

    public static event Action<GameMode> OnGameModeChanged; // 게임 모드가 변할 때 Invoke

    public void Init()
    {
        if (!PlayerPrefs.HasKey("MaxClearStage"))
        {
            PlayerPrefs.SetInt("MaxClearStage", -1);
            PlayerPrefs.Save();
        }

        MaxClearDay = PlayerPrefs.GetInt("MaxClearStage", -1);
    }

    private void ResetGameManager()
    {
        IsGameStopped = false;
        RunData.ResetData();
    }

    public void ResetGame()
    {
        ResetGameManager();
        // 필요시 다른 매니저들 리셋 호출
    }

    public void StartDay(int day)
    {
        CurrentDay = day;
        RunData.ResetData(); // 주행 데이터 초기화
    }

    public void ClearCurrentDay()
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