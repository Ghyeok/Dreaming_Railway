using System;
using System.Collections.Generic;
using UnityEngine;

// 역 생성을 담당하는 매니저
public class StationManager : SingletonManagers<StationManager>, IManager
{
    public bool isInitialized = false;

    public float minTravelTime = 10f;
    public float maxTravelTime = 15f;
    public float minStopTime = 6f;
    public float maxStopTime = 8f;

    public List<LineData> subwayLines = new List<LineData>();

    public int currentLineIdx; // 현재 노선 인덱스
    public int currentStationIdx; // 현재역 인덱스
    private int lastStationIdx = -1;

    public int passedStations;

    public static event Action OnStationStop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Init()
    {
        isInitialized = true;
        GenerateSubwayLines();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInitialized || !TransferManager.Instance.isInitialized) return;

        CheckCurrentStation();
        IsSubwayStopped();
    }

    public void ResetStationManager()
    {
        GenerateSubwayLines();
        currentLineIdx = 0;
        currentStationIdx = 0;
        passedStations = 0;
        ResetLastStationIdx();
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
    
    public float GetCurrentStationStoppingTime()
    {
        return subwayLines[currentLineIdx].stations[currentStationIdx].stopTime;
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
                        int transferStationIdx = UnityEngine.Random.Range(8, 11);
                        subwayLines[i].transferIdx = transferStationIdx;
                        subwayLines[i].hasDestination = false;
                        subwayLines[i].stations[transferStationIdx].stationType = StationType.Transfer;
                    }
                    else if (transferCnt >= 4 && transferCnt <= 6)
                    {
                        int transferStationIdx = UnityEngine.Random.Range(6, 9);
                        subwayLines[i].transferIdx = transferStationIdx;
                        subwayLines[i].hasDestination = false;
                        subwayLines[i].stations[transferStationIdx].stationType = StationType.Transfer;
                    }
                    else if (transferCnt >= 7)
                    {
                        int transferStationIdx = UnityEngine.Random.Range(4, 7);
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
                        int transferStationIdx = UnityEngine.Random.Range(8, 11);
                        subwayLines[i].transferIdx = transferStationIdx;
                        subwayLines[i].hasDestination = true;
                        subwayLines[i].stations[transferStationIdx].stationType = StationType.Destination;
                    }
                    else if (transferCnt >= 4 && transferCnt <= 6)
                    {
                        int transferStationIdx = UnityEngine.Random.Range(6, 9);
                        subwayLines[i].transferIdx = transferStationIdx;
                        subwayLines[i].hasDestination = true;
                        subwayLines[i].stations[transferStationIdx].stationType = StationType.Destination;
                    }
                    else if (transferCnt >= 7)
                    {
                        int transferStationIdx = UnityEngine.Random.Range(4, 7);
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
                        int transferStationIdx = UnityEngine.Random.Range(8, 11);
                        subwayLines[i].transferIdx = transferStationIdx;
                        subwayLines[i].hasDestination = false;
                        subwayLines[i].stations[transferStationIdx].stationType = StationType.Transfer;
                    }
                    else if (transferCnt >= 4 && transferCnt <= 6)
                    {
                        int transferStationIdx = UnityEngine.Random.Range(6, 9);
                        subwayLines[i].transferIdx = transferStationIdx;
                        subwayLines[i].hasDestination = false;
                        subwayLines[i].stations[transferStationIdx].stationType = StationType.Transfer;
                    }
                    else if (transferCnt >= 7)
                    {
                        int transferStationIdx = UnityEngine.Random.Range(4, 7);
                        subwayLines[i].transferIdx = transferStationIdx;
                        subwayLines[i].hasDestination = false;
                        subwayLines[i].stations[transferStationIdx].stationType = StationType.Transfer;
                    }
                    transferCnt++;
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

            if (timer.lineTime < accumulatedTime)
            {
                currentStationIdx = i;
                if (i != 0 && lastStationIdx < currentStationIdx)
                {
                    lastStationIdx = currentStationIdx;
                    passedStations++;
                }
                break;
            }
        }
    }

    public void ResetLastStationIdx()
    {
        lastStationIdx = -1;
    }

    public void IsSubwayStopped()
    {
        TimerManager timer = TimerManager.Instance;
        float tLine = timer.lineTime;
        float accumulatedTime = 0f;

        for (int i = 0; i <= subwayLines[currentLineIdx].transferIdx; i++)
        {
            accumulatedTime += subwayLines[currentLineIdx].stations[i].travelTime;

            if (i == currentStationIdx)
            {
                float stopStart = accumulatedTime;
                float stopEnd = accumulatedTime + subwayLines[currentLineIdx].stations[i].stopTime;

                if (timer.lineTime > stopStart && timer.lineTime < stopEnd)
                {
                    SubwayGameManager.Instance.isStopping = true;
                    OnStationStop?.Invoke();
                }
                else
                {
                    SubwayGameManager.Instance.isStopping = false;
                }
            }
            accumulatedTime += subwayLines[currentLineIdx].stations[i].stopTime;
        }
    }
}
