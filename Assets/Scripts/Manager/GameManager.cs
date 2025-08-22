using UnityEngine;

public class GameManager : SingletonManagers<GameManager>, IManager
{
    public enum GameState
    {
        Main,
        DaySelect,
        Subway,
        Dream,
    }

    public enum GameMode
    {
        None,
        Tutorial,
        Normal,
        Infinite,
    }

    public GameState gameState;
    public GameMode gameMode;

    public int infiniteCount;

    public bool isStopped;

    public void Init()
    {
        gameState = GameState.Main; // 메인에서 시작
    }

    public void ResetGame()
    {
        SubwayGameManager.Instance.ResetGameManager();
        StationManager.Instance.ResetStationManager();
        SubwayPlayerManager.Instance.ResetPlayerManager();
        TiredManager.Instance.ResetTiredManager();
        TimerManager.Instance.ResetTimer();
        TransferManager.Instance.ResetTransferManager();
        DreamManager.Instance.ResetDreamManager();
    }

    public void OnSelectInfiniteMode()
    {
        TransferManager.Instance.ResetTransferManager();
        StationManager.Instance.ResetStationManager();
    }

    public void StopGame()
    {
        Time.timeScale = 0f;
        isStopped = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isStopped = false;
    }
}
