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
                    maxTransferCount = 3;
                    break;
                case 2: // Day 2
                    maxTransferCount = 4;
                    break;
                case 3: // Day 3
                    maxTransferCount = 5;
                    break;
                case 4: // Day 4
                    maxTransferCount = 7;
                    break;
                case 5: // Day 5
                    maxTransferCount = 9;
                    break;
            }
        }
        else
        {
            maxTransferCount = 99;
        }
    }

    private void SuccessTransfer() // 환승 성공 시 1번만 실행
    {
        if (hasTransfered)
            return;

        if (StationManager.Instance.currentStationIdx == StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].transferIdx
            && TimerManager.Instance.stationTime >= StationManager.Instance.GetCurrentLineTotalTime()
            && !StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].hasDestination)
        {
            hasTransfered = true;

            if (DreamManager.Instance.isInDream) // 환승해야 하는데 꿈 속에 있으면 게임오버
            {
                SubwayGameManager.Instance.isGameOver = true;
                SubwayGameManager.Instance.GameOver();
                return;
            }

            SubwayGameManager.Instance.isStopping = false;
            Debug.Log("환승 성공!");

            curTransferCount++;
            TimerManager.Instance.stationTime = 0f;

            if (SubwayGameManager.Instance.isStandingCoolDown)
            {
                SubwayGameManager.Instance.standingCount++;
                if(SubwayGameManager.Instance.standingCount >= 3)
                {
                    SubwayGameManager.Instance.isStandingCoolDown = false;
                    SubwayGameManager.Instance.standingCount = 0;
                }
            }

            StationManager.Instance.currentLineIdx++;
            StationManager.Instance.currentStationIdx = 0;

            OnTransferSuccess?.Invoke();

            hasTransfered = false;
        }
    }

    public void ForceTransferByStanding()
    {
        SubwayGameManager.Instance.isStopping = false;
        Debug.Log("입석 성공!");

        curTransferCount++;
        SubwayGameManager.Instance.isStandingCoolDown = true; // 쿨다운 시작
        TimerManager.Instance.stationTime = 0f;
        StationManager.Instance.currentLineIdx++;
        StationManager.Instance.currentStationIdx = 0;

        SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.SLEEP;
        SubwayPlayerManager.Instance.playerBehave = SubwayPlayerManager.PlayerBehave.NONE;

        OnTransferSuccess?.Invoke();
    }

    private void SuccessGetOff() // 목적지 도착 성공시
    {
        if (hasArrived)
            return;

        if (StationManager.Instance.currentStationIdx == StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].transferIdx
            && TimerManager.Instance.stationTime >= StationManager.Instance.GetCurrentLineTotalTime()
            && StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].hasDestination
            && GameManager.Instance.gameState != GameManager.GameState.Dream)
        {
            hasArrived = true;

            SubwayGameManager.Instance.isStopping = false;
            StationManager.Instance.currentLineIdx = 0;
            Debug.Log("목적지 도착!");

            StageSelectManager.Instance.currentStage++;
            SceneManager.LoadScene("StageSelect");

            hasArrived = false; 
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestSubwayScene")
        {
            Debug.Log($"지하철 씬 로드 : {gameObject.name}");
            InitScene();
        }
    }
}
