using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StationDataInstance
{
    public StationData data;
    public float travelTime;
    public float stopTime;
    public StationType stationType;

    public StationDataInstance(StationData data) // 생성자
    {
        this.data = data;
        this.stationType = StationType.Normal;
        this.travelTime = Random.Range(data.minTravelTime, data.maxTravelTime);
        this.stopTime = Random.Range(data.minStopTime, data.maxStopTime);
    }
}

// 역 생성을 담당하는 매니저
public class StationManager : SingletonManagers<StationManager>
{
    public LineDataSO lineData;
    public List<StationDataInstance> stationDatas = new List<StationDataInstance>();
    public int transferStationIdx;

    public int currentStationIdx;
    public int destinationStationIdx;
    public float timeChecker;


    public override void Awake()
    {
        base.Awake();
        currentStationIdx = -1;
        timeChecker = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateStations();
    }

    // Update is called once per frame
    void Update()
    {
        CheckCurrentStation();
    }

    public void GenerateStations()
    {
        currentStationIdx = 0;
        stationDatas.Clear();

        foreach (var station in lineData.stations)
        {
            stationDatas.Add(new StationDataInstance(station));
        }

        ChooseStationType();
    }

    public void ChooseStationType()
    {
        // 환승역 결정
        if (TransferManager.Instance.curTransferCount < TransferManager.Instance.maxTransferCount)
        {
            if (SubwayGameManager.Instance.dayCount >= 1 && SubwayGameManager.Instance.dayCount <= 3)
            {
                transferStationIdx = Random.Range(10, 15);
                stationDatas[transferStationIdx].stationType = StationType.Transfer;
            }
            else if (SubwayGameManager.Instance.dayCount >= 4 && SubwayGameManager.Instance.dayCount <= 6)
            {
                transferStationIdx = Random.Range(6, 11);
                stationDatas[transferStationIdx].stationType = StationType.Transfer;
            }
            else if (SubwayGameManager.Instance.dayCount >= 7)
            {
                transferStationIdx = Random.Range(3, 7);
                stationDatas[transferStationIdx].stationType = StationType.Transfer;
            }
        }
        // 목적지 결정
        else if (TransferManager.Instance.curTransferCount == TransferManager.Instance.maxTransferCount)
        {
            if (SubwayGameManager.Instance.dayCount >= 1 && SubwayGameManager.Instance.dayCount <= 3)
            {
                destinationStationIdx = Random.Range(10, 15);
                stationDatas[destinationStationIdx].stationType = StationType.Destination;
            }
            else if (SubwayGameManager.Instance.dayCount >= 4 && SubwayGameManager.Instance.dayCount <= 6)
            {
                destinationStationIdx = Random.Range(6, 11);
                stationDatas[destinationStationIdx].stationType = StationType.Destination;
            }
            else if (SubwayGameManager.Instance.dayCount >= 7)
            {
                destinationStationIdx = Random.Range(3, 7);
                stationDatas[destinationStationIdx].stationType = StationType.Destination;
            }
        }
    }

    public void CheckCurrentStation() // 현재 어느 역을 지나고 있는지 확인하는 함수
    {
        Timer timer = SubwayGameManager.Instance.timer;

        if (timer.curTime >= timeChecker)
        {
            currentStationIdx++;
            timeChecker += stationDatas[currentStationIdx].travelTime;
            timeChecker += stationDatas[currentStationIdx].stopTime;
        }
    }
}
