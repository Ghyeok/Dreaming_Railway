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

    public bool isGameOverInDream;
    public bool isGameOverInSubway;

    public void Init()
    {
        gameState = GameState.Main; // 메인에서 시작
    }

    private void ResetGameManager()
    {
        infiniteCount = 0;
        isGameOverInDream = false;
        isGameOverInSubway = false;
    }

    public void ResetGame()
    {
        ResetGameManager();
        SubwayGameManager.Instance.ResetSubwayGameManager();
        StationManager.Instance.ResetStationManager();
        SubwayPlayerManager.Instance.ResetPlayerManager();
        TiredManager.Instance.ResetTiredManager();
        TimerManager.Instance.ResetTimer();
        TransferManager.Instance.ResetTransferManager();
        DreamManager.Instance.ResetDreamManager();
        TutorialManager.Instance.ResetTutorial();
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
