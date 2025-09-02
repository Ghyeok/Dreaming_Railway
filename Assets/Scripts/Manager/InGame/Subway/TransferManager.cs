using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


// 환승 로직을 담당하는 매니저
public class TransferManager : SingletonManagers<TransferManager>, IManager
{
    public bool isInitialized = false;

    public int curTransferCount;
    public int maxTransferCount;

    public static event Action OnTransferSuccess;

    private bool hasTransfered = false;
    private bool hasArrived = false;

    public void Init()
    {
        isInitialized = true;
        DetermineMaxTransferCount();
    }

    private void InitScene()
    {
        DetermineMaxTransferCount();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized || !StationManager.Instance.isInitialized) return;

        SuccessTransfer();
        SuccessGetOff();
    }

    public void ResetTransferManager()
    {
        curTransferCount = 0;
        DetermineMaxTransferCount();
    }

    private void DetermineMaxTransferCount()
    {
        if (GameManager.Instance.gameMode != GameManager.GameMode.Infinite)
        {
            switch (StageSelectManager.Instance.currentStage)
            {
                case 0: // 튜토리얼
                    maxTransferCount = 1;
                    break;
                case 1: // Day 1
                    maxTransferCount = 2;
                    break;
                case 2: // Day 2
                    maxTransferCount = 3;
                    break;
                case 3: // Day 3
                    maxTransferCount = 4;
                    break;
                case 4: // Day 4
                    maxTransferCount = 5;
                    break;
                case 5: // Day 5
                    maxTransferCount = 6;
                    break;
            }
        }
        else
        {
            maxTransferCount = 99;
        }
    }

    private float GetTimeToStationEnd(int lineIdx, int stationIdx)
    {
        var line = StationManager.Instance.subwayLines[lineIdx];
        float acc = 0f;
        for (int i = 0; i <= stationIdx; i++)
            acc += line.stations[i].travelTime + line.stations[i].stopTime;
        return acc;
    }

    private void SuccessTransfer()
    {
        // 1. 현재역이 환승역일때
        // 2. 현재 노선에서 흐른 시간이 현재 노선 전체 시간보다 커질때
        // 3. 도착역이 아닐때

        if (hasTransfered)
            return;

        int lineIdx = StationManager.Instance.currentLineIdx;
        var line = StationManager.Instance.subwayLines[lineIdx];

        bool atTransfer = (StationManager.Instance.currentStationIdx == line.transferIdx);
        bool timeReached = (TimerManager.Instance.lineTime >= GetTimeToStationEnd(lineIdx, line.transferIdx));
        bool notDestinationLine = !line.hasDestination;

        if (atTransfer && timeReached && notDestinationLine)
        {
            if (DreamManager.Instance.isInDream)
            {
                SubwayGameManager.Instance.isGameOver = true;
            }

            hasTransfered = true;
            curTransferCount++;
            TimerManager.Instance.lineTime = 0f;

            if (SubwayGameManager.Instance.isStandingCoolDown)
            {
                SubwayGameManager.Instance.standingCount++;
                if (SubwayGameManager.Instance.standingCount >= 2)
                {
                    SubwayGameManager.Instance.isStandingCoolDown = false;
                    SubwayGameManager.Instance.standingCount = 0;
                }
            }

            StationManager.Instance.currentLineIdx++;
            StationManager.Instance.currentStationIdx = 0;
            OnTransferSuccess?.Invoke();

            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "TestSubwayScene")
            {
                Animator anim = SubwayPlayerManager.Instance.subwayPlayer.gameObject.GetComponent<Animator>();
                anim.SetTrigger("isTransfer");
            }
            
            hasTransfered = false;
        }
    }

    private void SuccessGetOff()
    {
        // 1. 현재역이 도착역일때
        // 2. 현재 노선에서 흐른 시간이 현재 노선 전체 시간보다 커질때
        // 3. 꿈 속이 아닐때

        if (hasArrived)
            return;

        int lineIdx = StationManager.Instance.currentLineIdx;
        var lines = StationManager.Instance.subwayLines;
        if (lines == null || lineIdx < 0 || lineIdx >= lines.Count) return;

        var line = lines[lineIdx];

        bool atDest = (StationManager.Instance.currentStationIdx == line.transferIdx);
        bool timeReached = (TimerManager.Instance.lineTime >= GetTimeToStationEnd(lineIdx, line.transferIdx));
        bool isDestinationLine = line.hasDestination;

        if (atDest && timeReached && isDestinationLine)
        {
            if (DreamManager.Instance.isInDream)
            {
                SubwayGameManager.Instance.isGameOver = true;
                return;
            }

            if (SubwayGameManager.Instance.isGameOver)
                return;

            hasArrived = true;

            StationManager.Instance.currentLineIdx = 0;
            StationManager.Instance.ResetLastStationIdx();
            StageSelectManager.Instance.maxClearStage++;

            GameManager.Instance.gameState = GameManager.GameState.DaySelect;
            UIManager.Instance.ShowPopupUI<UI_GameClearPopup>("UI_GameClearPopup");

            hasArrived = false;
        }
    }

    public void ForceTransferByStanding()
    {
        SubwayGameManager.Instance.isStopping = false;
        Debug.Log("입석 성공!");

        curTransferCount++;
        SubwayGameManager.Instance.isStandingCoolDown = true; // 쿨다운 시작
        TimerManager.Instance.lineTime = 0f;
        StationManager.Instance.currentLineIdx++;
        StationManager.Instance.currentStationIdx = 0;
        StationManager.Instance.ResetLastStationIdx();

        SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.SLEEP;
        SubwayPlayerManager.Instance.playerBehave = SubwayPlayerManager.PlayerBehave.NONE;

        OnTransferSuccess?.Invoke();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        //OnTransferSuccess += StationManager.Instance.CheckLastLine;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //OnTransferSuccess -= StationManager.Instance.CheckLastLine;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestSubwayScene")
        {
            InitScene();
        }
    }
}
