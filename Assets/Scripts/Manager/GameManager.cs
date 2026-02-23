using System;
using UnityEngine;

public class GameManager : SingletonManagers<GameManager>, IManager
{
    public GameState GameState { get; private set; }
    public GameMode GameMode { get; private set; }
    public bool IsStopped { get; private set; }

    public static event Action<GameState> OnGameStateChanged;
    public static event Action<GameMode> OnGameModeChanged;

    public void Init()
    {
        // 메인에서 시작
        this.GameState = GameState.Main;

        if (!PlayerPrefs.HasKey("MaxClearStage"))
        {
            PlayerPrefs.SetInt("MaxClearStage", -1);
            PlayerPrefs.Save();
        }
    }

    private void ResetGameManager()
    {

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
        ScriptManager.Instance.ResetScript();
    }

    public void ChangeGameState(GameState newState)
    {
        if (GameState == newState) return;

        GameState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    // 게임 모드 변경
    public void ChangeGameMode(GameMode newMode)
    {
        if (GameMode == newMode) return;

        GameMode = newMode;
        OnGameModeChanged?.Invoke(newMode);
    }

    public void OnSelectInfiniteMode()
    {
        TransferManager.Instance.ResetTransferManager();
        StationManager.Instance.ResetStationManager();
    }

    public void StopGame()
    {
        Time.timeScale = 0f;
        IsStopped = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsStopped = false;
    }

    public void GameOverOrClear()
    {
        // 1. 배경 움직임 멈춤
        // 2. 플레이어 애니메이션 멈춤
        // 3. 시간 멈춤
        // 4. 피로도 증가 멈춤

        // 1. BackgroundScroller에서 이벤트 구독으로 처리
        // 2. UI_SubwayScene에서 이벤트 구독으로 처리
        // 3. 시간 멈춤
        TimerManager.Instance.StopTimer();
        // 4. 피로도는 SubwayGameManager.Instance.isGameOver가 true면 증가하지 않음
        SubwayGameManager.Instance.isGameOver = true;
    }

    private void OnEnable()
    {
        SubwayGameManager.OnSubwayGameOver += GameOverOrClear;
        TransferManager.OnGetOffSuccess += GameOverOrClear;
        FogMovement.OnDreamGameOver += GameOverOrClear;
    }

    private void OnDisable()
    {
        SubwayGameManager.OnSubwayGameOver -= GameOverOrClear;
        TransferManager.OnGetOffSuccess -= GameOverOrClear;
        FogMovement.OnDreamGameOver -= GameOverOrClear;
    }
}
