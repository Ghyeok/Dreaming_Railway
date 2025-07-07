using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


// 환승 로직을 담당하는 매니저
public class TransferManager : SingletonManagers<TransferManager>
{
    public int curTransferCount;
    public int maxTransferCount;

    public static event Action OnTransferSuccess;

    private bool hasTransfered = false;
    private bool hasArrived = false;

    private void InitScene()
    {
        DetermineMaxTransferCount();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DetermineMaxTransferCount();
        SuccessTransfer();
    }

    public void ResetTransferManager()
    {
        curTransferCount = 0;
    }

    private void DetermineMaxTransferCount()
    {
        switch (SubwayGameManager.Instance.dayCount)
        {
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
            }

            SubwayGameManager.Instance.isStopping = false;
            Debug.Log("환승 성공!");

            curTransferCount++;
            TimerManager.Instance.stationTime = 0f;
            SubwayGameManager.Instance.standingCount++;
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
        TimerManager.Instance.stationTime = 0f;
        StationManager.Instance.currentLineIdx++;
        StationManager.Instance.currentStationIdx = 0;

        SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.SLEEP;
        SubwayPlayerManager.Instance.playerBehave = SubwayPlayerManager.PlayerBehave.NONE;

        OnTransferSuccess?.Invoke();
    }

    private void SuccessGetOff(PointerEventData data) // 목적지 도착 성공시
    {
        if (hasArrived)
            return;

        if (StationManager.Instance.currentStationIdx == StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].transferIdx
            && SubwayGameManager.Instance.isStopping && StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].hasDestination)
        {
            hasArrived = true;

            SubwayGameManager.Instance.isStopping = false;
            SubwayGameManager.Instance.dayCount++;
            StationManager.Instance.currentLineIdx = 0;
            Debug.Log("목적지 도착!");

            StationManager.Instance.GenerateSubwayLines(); // Day가 바뀌면서 새로운 노선 생성
        }
        else
        {
            Debug.Log("목적지 도착 실패..");
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
