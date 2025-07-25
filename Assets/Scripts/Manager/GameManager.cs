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

    public void Init()
    {
        UIManager.Instance.ShowSceneUI<UI_Scene>("UI_MainMenuScene");
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
    }

    public void OnSelectInfiniteMode()
    {
        TransferManager.Instance.ResetTransferManager();
        StationManager.Instance.ResetStationManager();
    }
}
