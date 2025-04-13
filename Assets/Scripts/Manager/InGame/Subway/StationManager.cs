using System.Collections.Generic;
using UnityEngine;

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
    public int stationIdx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateStations();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateStations()
    {
        stationDatas.Clear();

        foreach (var station in lineData.stations)
        {
            stationDatas.Add(new StationDataInstance(station));
        }

        ChooseTransferStation();
    }

    public void ChooseTransferStation()
    {
        if (TransferManager.Instance.dayCount >= 1 && TransferManager.Instance.dayCount <= 3)
        {
            stationIdx = Random.Range(10, 15);
            stationDatas[stationIdx].stationType = StationType.Transfer;
        }
        else if (TransferManager.Instance.dayCount >= 4 && TransferManager.Instance.dayCount <= 6)
        {
            stationIdx = Random.Range(6, 11);
            stationDatas[stationIdx].stationType = StationType.Transfer;
        }
        else if (TransferManager.Instance.dayCount >= 7)
        {
            stationIdx = Random.Range(3, 7);
            stationDatas[stationIdx].stationType = StationType.Transfer;
        }
    }

    public void ChooseDestinationStation()
    {
        // 마지막 환승 카운트라면 목적지 역 선택
    }
}
