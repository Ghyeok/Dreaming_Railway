using NUnit.Framework;
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

    private bool hasTransfered = false;
    private bool hasArrived = false;

    private void InitScene()
    {
        curTransferCount = 0;
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
    
    public void SuccessTransfer() // 환승 성공 시 1번만 실행
    {
        if (hasTransfered)
            return;

        if (StationManager.Instance.currentStationIdx == StationManager.Instance.transferStationIdx &&
            SubwayGameManager.Instance.isStopping)
        {
            hasTransfered = true;

            SubwayGameManager.Instance.isStopping = false;
            curTransferCount++;
            Debug.Log("환승 성공!");
            StationManager.Instance.GenerateStations();
        }
    }

    public void SuccessGetOff(PointerEventData data) // 목적지 도착 성공시
    {
        if (hasArrived)
            return;

        if (StationManager.Instance.currentStationIdx == StationManager.Instance.destinationStationIdx &&
            SubwayGameManager.Instance.isStopping)
        {
            hasArrived = true;

            SubwayGameManager.Instance.isStopping = false;
            SubwayGameManager.Instance.dayCount++;
            Debug.Log("목적지 도착!");
            StationManager.Instance.GenerateStations();
        }
        else
        {
            Debug.Log("목적지 도착 실패..");
        }
    }

    public void CheckFailTransfer()
    {
        if (StationManager.Instance.currentStationIdx > StationManager.Instance.transferStationIdx)
        {
            Debug.Log("환승 실패! 게임 오버");
            SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.GAMEOVER;
        }
    }

    public void ReturnTransferState()
    {
        hasTransfered = false;
    }

    public void ReturnArriveState()
    {
        hasArrived = false;
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
