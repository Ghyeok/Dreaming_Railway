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

    public TextMeshProUGUI transferText;

    public override void Awake()
    {
        base.Awake();
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
    }

    void DetermineMaxTransferCount()
    {
        switch (SubwayGameManager.Instance.dayCount)
        {
            case 1:
                maxTransferCount = 3;
                break;
            case 2:
                maxTransferCount = 4;
                break;
            case 3:
                maxTransferCount = 5;
                break;
            case 4:
                maxTransferCount = 7;
                break;
            case 5:
                maxTransferCount = 9;
                break;
        }
    }

    public void SuccessTransfer(PointerEventData data) // 환승 성공 시
    {
        if (StationManager.Instance.currentStationIdx == StationManager.Instance.transferStationIdx)
        {
            curTransferCount++;
            Debug.Log("환승 성공!");
            StationManager.Instance.GenerateStations();
        }
        else
        {
            Debug.Log("환승 실패..");
        }
    }

    public void SuccessGetOff(PointerEventData data) // 목적지 도착 성공시
    {
        if (StationManager.Instance.currentStationIdx == StationManager.Instance.destinationStationIdx)
        {
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
}
