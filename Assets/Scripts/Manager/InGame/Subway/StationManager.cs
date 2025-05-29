using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateStations(); // 다른 매니저들 생성 전에 실행되면 안되므로, Start()에 넣어야한다
    }

    private void InitScene()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckCurrentStation();
        IsSubwayStopped();
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
        TransferManager.Instance.ReturnTransferState();
        SubwayGameManager.Instance.timer.stationTime = 0f;
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
        // 목적지역 결정
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
        float accumulatedTime = 0f; 

        for (int i = 0; i < stationDatas.Count; i++)
        {
            accumulatedTime += stationDatas[i].travelTime;
            accumulatedTime += stationDatas[i].stopTime;

            if (timer.stationTime < accumulatedTime)
            {
                currentStationIdx = i;
                break;
            }
        }
    }

    public void IsSubwayStopped()
    {
        Timer timer = SubwayGameManager.Instance.timer;
        float accumulatedTime = 0f;

        for (int i = 0; i < stationDatas.Count; i++)
        {
            accumulatedTime += stationDatas[i].travelTime;

            if (i == currentStationIdx)
            {
                float stopStart = accumulatedTime;
                float stopEnd = accumulatedTime + stationDatas[i].stopTime;

                SubwayGameManager.Instance.isStopping = (timer.curTime > stopStart && timer.curTime < stopEnd);
            }
            accumulatedTime += stationDatas[i].stopTime;
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
