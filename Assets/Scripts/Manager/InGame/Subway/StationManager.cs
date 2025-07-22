using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 역 생성을 담당하는 매니저
public class StationManager : SingletonManagers<StationManager>
{
    public float minTravelTime = 10f;
    public float maxTravelTime = 15f;
    public float minStopTime = 6f;
    public float maxStopTime = 8f;

    public List<LineData> subwayLines = new List<LineData>();

    public int currentLineIdx; // 현재 노선 인덱스
    public int currentStationIdx; // 현재역 인덱스

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GenerateSubwayLines();
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

    public void ResetStationManager()
    {
        GenerateSubwayLines();
        currentLineIdx = 0;
        currentStationIdx = 0;
    }

    public void GenerateSubwayLines()
    {
        subwayLines.Clear();

        int lineCount = 0;
        lineCount = TransferManager.Instance.maxTransferCount + 1;

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

    public float GetCurrentLineTotalTime()
    {
        float total = 0f;
        var stations = subwayLines[currentLineIdx].stations;
        int lastIdx = subwayLines[currentLineIdx].transferIdx;

        for (int i = 0; i <= lastIdx; i++)
        {
            total += stations[i].travelTime;
            total += stations[i].stopTime;
        }

        return total;
    }

    private void ChooseStationType()
    {
        int transferCnt = 0;

        if (GameManager.Instance.gameMode != GameManager.GameMode.Infinite)
        {
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
        else if (GameManager.Instance.gameMode == GameManager.GameMode.Infinite)
        {
            for (int i = 0; i < subwayLines.Count; i++)
            {
                if (i < subwayLines.Count - 1)
                {
                    if (transferCnt >= 0 && transferCnt <= 3)
                    {
                        int transferStationIdx = Random.Range(8, 13);
                        subwayLines[i].transferIdx = transferStationIdx;
                        subwayLines[i].hasDestination = false;
                        subwayLines[i].stations[transferStationIdx].stationType = StationType.Transfer;
                    }
                    else if (transferCnt >= 4 && transferCnt <= 6)
                    {
                        int transferStationIdx = Random.Range(5, 9);
                        subwayLines[i].transferIdx = transferStationIdx;
                        subwayLines[i].hasDestination = false;
                        subwayLines[i].stations[transferStationIdx].stationType = StationType.Transfer;
                    }
                    else if (transferCnt >= 7)
                    {
                        int transferStationIdx = Random.Range(2, 6);
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
    }

    public void CheckCurrentStation() // 현재 어느 역을 지나고 있는지 확인하는 함수
    {
        TimerManager timer = TimerManager.Instance;

        float accumulatedTime = 0f;

        for (int i = 0; i <= subwayLines[currentLineIdx].transferIdx; i++)
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
        TimerManager timer = TimerManager.Instance;
        float accumulatedTime = 0f;

        for (int i = 0; i <= subwayLines[currentLineIdx].transferIdx; i++)
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
