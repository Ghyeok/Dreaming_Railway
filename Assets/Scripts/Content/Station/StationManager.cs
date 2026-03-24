using System;
using System.Collections.Generic;
using UnityEngine;

// 싱글톤 상속 X, ITickable 규격 준수
public class StationManager : MonoBehaviour, ITickable
{
    // 씬 컨텍스트 (게시판) 참조
    private SubwaySceneRefs _refs;

    [Header("Time Settings")]
    public float minTravelTime = 10f;
    public float maxTravelTime = 15f;
    public float minStopTime = 6f;
    public float maxStopTime = 8f;

    [Header("Subway Data")]
    public List<SubwayLineData> subwayLines = new List<SubwayLineData>();

    public int currentLineIdx; // 현재 노선 인덱스
    public int currentStationIdx; // 현재 역 인덱스
    public int passedStations; // 지금까지 지나온 역의 개수

    public float CurrentLineTime { get; private set; } // 현재 노선 타이머
    public bool IsStopping { get; private set; } // 정차 구간을 지나는 중인가?

    // 다음 상태(정차/출발)까지 남은 시간을 재는 타이머
    private float _timeToNextState;

    public event Action OnStationStop; // 정차 시간에 진입한 순간
    public event Action OnStationDeparted; // 완전히 정차한 순간
    public event Action OnLineEnded; // 노선이 완전히 끝난 순간

    // SubwaySceneBinder가 호출하여 초기화
    public void Init(SubwaySceneRefs refs)
    {
        _refs = refs;
        int maxTransfer = refs.transferManager.maxTransferCount;
        GenerateSubwayLines(maxTransfer);

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.Register(this);
        }

        // 첫 번째 역으로 가는 이동 시간 세팅
        if (subwayLines.Count > 0 && subwayLines[0].stations.Count > 0)
        {
            _timeToNextState = subwayLines[currentLineIdx].stations[0].travelTime;
        }
    }

    public void Tick(float deltaTime)
    {
        float dt = deltaTime;
        CurrentLineTime += dt;
        _timeToNextState -= dt;

        // 남은 시간이 0 이하가 되면 상태 전환!
        if (_timeToNextState <= 0f)
        {
            AdvanceSubwayState();
        }
    }

    private void AdvanceSubwayState()
    {
        SubwayLineData line = subwayLines[currentLineIdx];

        if (!IsStopping)
        {
            // [주행 -> 정차] 역에 도착함
            IsStopping = true;
            _timeToNextState += line.stations[currentStationIdx].stopTime;
            OnStationStop?.Invoke();
        }
        else
        {
            // [정차 -> 출발] 다음 동작 판별
            IsStopping = false;

            // 현재 역이 이 노선의 마지막 역(환승역/종착역)인가?
            if (currentStationIdx == line.transferIdx)
            {
                // 노선 종료, 환승 타이밍
                OnLineEnded?.Invoke();
            }
            else
            {
                // 다음 역으로 주행 시작
                currentStationIdx++;
                _timeToNextState += line.stations[currentStationIdx].travelTime;
                OnStationDeparted?.Invoke();
            }
        }
    }

    public void ResetStationManager()
    {
        int maxTransfer = _refs.transferManager.maxTransferCount;
        GenerateSubwayLines(maxTransfer);

        currentLineIdx = 0;
        currentStationIdx = 0;
        passedStations = 0;
        CurrentLineTime = 0f;
        IsStopping = false;

        if (subwayLines.Count > 0 && subwayLines[0].stations.Count > 0)
        {
            _timeToNextState = subwayLines[currentLineIdx].stations[currentStationIdx].travelTime;
        }
    }

    public void GenerateSubwayLines(int maxTransferCount)
    {
        subwayLines.Clear();

        int lineCount = maxTransferCount + 1;
        int stationPerLine = 20; // 넉넉하게 생성

        for (int i = 0; i < lineCount; i++)
        {
            SubwayLineData newLine = new SubwayLineData();
            for (int j = 0; j < stationPerLine; j++)
            {
                newLine.stations.Add(new StationData(minTravelTime, maxTravelTime, minStopTime, maxStopTime));
            }
            subwayLines.Add(newLine);
        }

        ChooseStationType();
    }

    private void ChooseStationType()
    {
        bool isNormalMode = GameManager.Instance.GameMode == GameMode.NormalMode;

        for (int i = 0; i < subwayLines.Count; i++)
        {
            bool isDestinationLine = isNormalMode && (i == subwayLines.Count - 1);
            int transferStationIdx;

            if (i <= 3) transferStationIdx = UnityEngine.Random.Range(8, 11);
            else if (i <= 6) transferStationIdx = UnityEngine.Random.Range(6, 9);
            else transferStationIdx = UnityEngine.Random.Range(4, 7);

            subwayLines[i].transferIdx = transferStationIdx;
            subwayLines[i].hasDestination = isDestinationLine;

            StationType type = isDestinationLine ? StationType.Destination : StationType.Transfer;
            subwayLines[i].stations[transferStationIdx].stationType = type;
        }
    }
}