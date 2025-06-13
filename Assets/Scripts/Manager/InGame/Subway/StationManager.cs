using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 역 생성을 담당하는 매니저
public class StationManager : SingletonManagers<StationManager>
{
    public float minTravelTime = 20f;
    public float maxTravelTime = 30f;
    public float minStopTime = 7f;
    public float maxStopTime = 9f;

    public List<LineData> subwayLines = new List<LineData>();

    public int currentLineIdx; // 현재 노선 인덱스
    public int currentStationIdx; // 현재역 인덱스

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateSubwayLines();
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

    public void GenerateSubwayLines()
    {
        subwayLines.Clear();

        int lineCount = TransferManager.Instance.maxTransferCount + 1;
        int stationPerLine = 20; // 넉넉하게

        for (int i = 0; i < lineCount; i++)
        {
            LineData newLine = new LineData();

            for (int j = 0; j < stationPerLine; j++)
            {
                StationData station = new StationData(minTravelTime, maxTravelTime, minStopTime, maxStopTime);
                newLine.stations.Add(station);
            }
            subwayLines.Add(newLine);
        }
        ChooseStationType();
    }

    private void ChooseStationType()
    {
        int transferCnt = 0;
        for (int i = 0; i < subwayLines.Count; i++)
        {
            if (i < subwayLines.Count - 1)
            {
                if (transferCnt >= 0 && transferCnt <= 3)
                {
                    int transferStationIdx = Random.Range(10, 15);
                    subwayLines[i].transferIdx = transferStationIdx;
                    subwayLines[i].hasDestination = false;
                    subwayLines[i].stations[transferStationIdx].stationType = StationType.Transfer;
                }
                else if (transferCnt >= 4 && transferCnt <= 6)
                {
                    int transferStationIdx = Random.Range(6, 11);
                    subwayLines[i].transferIdx = transferStationIdx;
                    subwayLines[i].hasDestination = false;
                    subwayLines[i].stations[transferStationIdx].stationType = StationType.Transfer;
                }
                else if (transferCnt >= 7)
                {
                    int transferStationIdx = Random.Range(3, 7);
                    subwayLines[i].transferIdx = transferStationIdx;
                    subwayLines[i].hasDestination = false;
                    subwayLines[i].stations[transferStationIdx].stationType = StationType.Transfer;
                }
                transferCnt++;
            }
            else if (i == subwayLines.Count - 1)
            {
                if (transferCnt >= 0 && transferCnt <= 3)
                {
                    int transferStationIdx = Random.Range(10, 15);
                    subwayLines[i].transferIdx = transferStationIdx;
                    subwayLines[i].hasDestination = true;
                    subwayLines[i].stations[transferStationIdx].stationType = StationType.Destination;
                }
                else if (transferCnt >= 4 && transferCnt <= 6)
                {
                    int transferStationIdx = Random.Range(6, 11);
                    subwayLines[i].transferIdx = transferStationIdx;
                    subwayLines[i].hasDestination = true;
                    subwayLines[i].stations[transferStationIdx].stationType = StationType.Destination;
                }
                else if (transferCnt >= 7)
                {
                    int transferStationIdx = Random.Range(3, 7);
                    subwayLines[i].transferIdx = transferStationIdx;
                    subwayLines[i].hasDestination = true;
                    subwayLines[i].stations[transferStationIdx].stationType = StationType.Destination;
                }
            }
        }
    }

    public void CheckCurrentStation() // 현재 어느 역을 지나고 있는지 확인하는 함수
    {
        Timer timer = SubwayGameManager.Instance.timer;
        float accumulatedTime = 0f;

        for (int i = 0; i < subwayLines[currentLineIdx].transferIdx; i++)
        {
            accumulatedTime += subwayLines[currentLineIdx].stations[i].travelTime;
            accumulatedTime += subwayLines[currentLineIdx].stations[i].stopTime;

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

        for (int i = 0; i < subwayLines[currentLineIdx].transferIdx; i++)
        {
            accumulatedTime += subwayLines[currentLineIdx].stations[i].travelTime;

            if (i == currentStationIdx)
            {
                float stopStart = accumulatedTime;
                float stopEnd = accumulatedTime + subwayLines[currentLineIdx].stations[i].stopTime;

                SubwayGameManager.Instance.isStopping = (timer.curTime > stopStart && timer.curTime < stopEnd);
            }
            accumulatedTime += subwayLines[currentLineIdx].stations[i].stopTime;
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
