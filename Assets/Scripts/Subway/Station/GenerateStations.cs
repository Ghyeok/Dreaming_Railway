using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StationDataInstance
{
    public StationData data;
    public float travelTime;
    public float stopTime;
    public StationType stationType;

    public StationDataInstance(StationData data)
    {
        this.data = data;
        this.stationType = StationType.Normal;
        this.travelTime = Random.Range(data.minTravelTime, data.maxTravelTime);
        this.stopTime = Random.Range(data.minStopTime, data.maxStopTime);
    }
}

public class GenerateStations : MonoBehaviour
{
    public LineDataSO lineData;

    public List<StationDataInstance> stationDatas = new List<StationDataInstance>();

    public int stationIdx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stationDatas.Clear();

        foreach(var station in lineData.stations)
        {
            stationDatas.Add(new StationDataInstance(station));
        }

        ChooseTransferStation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChooseTransferStation()
    {
        // if (환승 1~3회) { stationIdx = Random.Range(10,15); stationDatas[stationIdx].stationType = StationType.Transfer;}
        // ...
        stationIdx = Random.Range(10, 15);
        stationDatas[stationIdx].stationType = StationType.Transfer;
    }

    public void ChooseDestinationStation()
    {
        // 마지막 환승 카운트라면 목적지 역 선택
    }
}
