using System;
using System.Collections.Generic;
using UnityEngine;

public class SubwayFlowManager : SingletonManagers<SubwayFlowManager>, ITickable, IManager
{
    [Header("Subway Data")]
    [field: SerializeField] public List<SubwayLineData> SubwayLines { get; private set; } = new List<SubwayLineData>(); // 전체 노선 데이터

    [Header("Line&Station Datas")]
    public int currentLineIdx; // 현재 노선 인덱스
    public int currentStationIdx; // 현재 역 인덱스
    public int passedStations; // 지금까지 지나온 역의 개수
    [field:SerializeField] public float CurrentLineTime { get; private set; } // 현재 노선 타이머
    [field: SerializeField] public bool IsStopping { get; private set; } // 정차 구간을 지나는 중인가?

    // 다음 상태(정차/출발)까지 남은 시간을 재는 타이머
    private float _timeToNextState;

    public event Action OnStationEnterStop; // 정차 시간에 진입한 순간
    public event Action OnStationDeparted; // 완전히 정차한 순간
    public event Action OnStationDepart; // 정차하고 다시 출발하는 순간
    public event Action OnLineEnded; // 노선이 완전히 끝난 순간(환승 or 도착 타이밍)

    public void Init()
    {
        if(TimerManager.Instance != null)
        {
            TimerManager.Instance.Register(this);
        }
    }

    public void SetLineData(List<SubwayLineData> generatedLines)
    {
        SubwayLines = generatedLines;
        ResetFlow();
    }

    public void ResetFlow()
    {
        currentLineIdx = 0;
        currentStationIdx = 0;
        passedStations = 0;
        CurrentLineTime = 0f;
        IsStopping = false;

        // 초기 상태 세팅
        if (SubwayLines.Count > 0 && SubwayLines[0].stations.Count > 0)
        {
            _timeToNextState = SubwayLines[currentLineIdx].stations[0].travelTime;
        }
    }

    // 다음 노선으로 이동
    public void GoToNextLine()
    {
        currentLineIdx++;
        currentStationIdx = 0;
        CurrentLineTime = 0f;
        IsStopping = false;

        if (currentLineIdx < SubwayLines.Count && SubwayLines[currentLineIdx].stations.Count > 0)
        {
            _timeToNextState = SubwayLines[currentLineIdx].stations[0].travelTime;
        }
    }

    public void Tick(float deltaTime)
    {
        if (GameManager.Instance.IsGameStopped || SubwayLines.Count == 0) return;

        CurrentLineTime += deltaTime;
        _timeToNextState -= deltaTime;

        while (_timeToNextState <= 0f) // _timeToNextState가 0보다 작으면 구간이 바뀌어야함
        {
            AdvanceSubwayState();
        }
    }

    private void SetFlowSpeed(bool isDream)
    {

    }

    private void AdvanceSubwayState()
    {
        SubwayLineData line = SubwayLines[currentLineIdx]; // 현재 노선

        if (!IsStopping) // 정차 구간이 아니면
        {
            IsStopping = true;
            _timeToNextState += line.stations[currentStationIdx].stopTime;
            OnStationEnterStop?.Invoke();
        }
        else // 정차 구간이면
        {
            IsStopping = false;

            if (currentStationIdx == line.transferIdx) // 노선의 마지막 역
            {
                OnLineEnded?.Invoke();
            }
            else 
            {
                currentStationIdx++;
                passedStations++;
                _timeToNextState += line.stations[currentStationIdx].travelTime;

                OnStationDeparted?.Invoke();
            }
        }
    }

}
