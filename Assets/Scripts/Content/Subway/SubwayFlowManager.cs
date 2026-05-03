using System;
using System.Collections.Generic;
using UnityEngine;

public class SubwayFlowManager : SingletonManagers<SubwayFlowManager>, ITickable, IManager
{
    // 시간 흐름 속도
    private const float NORMAL_FLOW_SPEED = 1f;
    private const float DREAM_MIN_FLOW_SPEED = 3.1f;
    private const float DREAM_MAX_FLOW_SPEED = 4.1f;

    // 무한 모드 최대 환승 횟수 (무제한)
    private const int INFINITE_TRANSFER_COUNT = 999;

    // 꿈 맵 길이 결정 임계값 (초 단위)
    private const float AWAKE_TIME_TIER1_MAX = 50f;
    private const float AWAKE_TIME_TIER2_MAX = 100f;
    private const float AWAKE_TIME_TIER3_MAX = 125f;

    [Header("Subway Data")]
    [field: SerializeField] public List<LineData> SubwayLines { get; private set; } = new List<LineData>(); // 전체 노선 데이터

    [Header("Line & Station Datas")]
    [field: SerializeField] public int CurrentLineIdx { get; private set; } = 0; // 현재 노선 인덱스
    [field: SerializeField] public int CurrentStationIdx { get; private set; } = 0; // 현재 역 인덱스
    [field: SerializeField] public int PassedStations { get; private set; } = 0; // 지금까지 지나온 역의 개수
    [field: SerializeField] public float CurrentLineTime { get; private set; } = 0; // 현재 노선 타이머
    [field: SerializeField] public bool IsStopping { get; private set; } = false; // 정차 구간인가?

    [Header("Transfer Datas")]
    [field: SerializeField] public int CurTransferCount { get; private set; } = 0; // 현재 환승 횟수
    [field: SerializeField] public int MaxTransferCount { get; private set; } = 0;// 최대 환승 횟수
    [field: SerializeField] public bool IsTransferRecently { get; private set; } = false; // 최근에 환승했는지 체크 -> 배경 스크롤 용
    [field: SerializeField] public bool IsMissedTransferStation { get; private set; } = false; // 환승역을 놓쳤는지? (게임오버)

    // 다음 상태(정차/출발)까지 남은 시간을 재는 타이머
    [SerializeField] private float _timeToNextState = 0f;
    [SerializeField] private float flowSpeed = 1f;

    public event Action<float> OnTimeUpdated; // 타이머가 업데이트 되는 순간 -> Tick

    public event Action OnStationEnterStopInterval; // 정차 구간에 진입한 순간
    public event Action OnStationCompleteDepart; // 완전히 정차한 순간
    public event Action OnStationStartDepart; // 정차하고 다시 출발하는 순간
    public event Action OnLineEnded; // 노선이 완전히 끝난 순간 (환승 or 도착 타이밍)
    public event Action OnDayCleared; // Day 클리어 순간 (마지막 환승 완료)

    public void Init()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.Register(this);
        }
    }

    public void SetLineData(List<LineData> generatedLines)
    {
        SubwayLines = generatedLines;
        ResetSubwayGame();
    }

    /// <summary>
    /// 지하철 게임이 새로 시작하는 순간 초기 값 세팅
    /// </summary>
    public void ResetSubwayGame()
    {
        CurrentLineIdx = 0;
        CurrentStationIdx = 0;
        PassedStations = 0;
        CurrentLineTime = 0f;
        IsStopping = false;

        CurTransferCount = 0;
        MaxTransferCount = DetermineMaxTransferCount();
        IsTransferRecently = false;
        IsMissedTransferStation = false;

        flowSpeed = 1f;

        // 초기 상태 세팅
        if (SubwayLines.Count > 0 && SubwayLines[0].stations.Count > 0)
        {
            _timeToNextState = SubwayLines[CurrentLineIdx].stations[0].travelTime;
        }
    }

    /// <summary>
    /// 다음 노선으로 이동, 환승이냐 도착이냐에 따라 로직 분기
    /// </summary>
    private void GoToNextLine()
    {
        CurTransferCount++;
        CurrentLineIdx++;

        if (CurTransferCount >= MaxTransferCount) // 노선의 끝, Day 종료
        {
            GameManager.Instance.UpdateMaxClearDay();
            OnDayCleared?.Invoke();
        }
        else // 다음 노선으로 이동
        {
            CurrentStationIdx = 0;
            CurrentLineTime = 0f;
            IsStopping = false;

            if (CurrentLineIdx < SubwayLines.Count && SubwayLines[CurrentLineIdx].stations.Count > 0)
            {
                _timeToNextState = SubwayLines[CurrentLineIdx].stations[0].travelTime;
            }
        }
    }

    private void GoToNextStation()
    {
        IsStopping = false;
        CurrentStationIdx++;
        PassedStations++;
        _timeToNextState += SubwayLines[CurrentLineIdx].stations[CurrentStationIdx].travelTime;
    }

    public void Tick(float deltaTime)
    {
        if (GameManager.Instance.IsGameStopped || SubwayLines.Count == 0) return;

        CurrentLineTime += deltaTime * flowSpeed;
        OnTimeUpdated?.Invoke(CurrentLineTime);

        _timeToNextState -= deltaTime * flowSpeed;

        while (_timeToNextState <= 0f) // _timeToNextState가 0보다 작으면 구간이 바뀌어야함
        {
            AdvanceSubwayState();
        }
    }

    public void SetFlowSpeed(bool isDream)
    {
        if (isDream)
        {
            flowSpeed = UnityEngine.Random.Range(DREAM_MIN_FLOW_SPEED, DREAM_MAX_FLOW_SPEED);
        }
        else
        {
            flowSpeed = NORMAL_FLOW_SPEED;
        }
    }

    private void AdvanceSubwayState()
    {
        LineData line = SubwayLines[CurrentLineIdx]; // 현재 노선

        if (!IsStopping) // 정차 구간이 아니면 정차 구간 진입
        {
            IsStopping = true;
            _timeToNextState += line.stations[CurrentStationIdx].stopTime;

            OnStationEnterStopInterval?.Invoke(); // 정차 구간 진입하는 순간
        }
        else // 정차 구간이면
        {
            if (CurrentStationIdx == line.transferIdx) // 노선의 마지막 역
            {
                GoToNextLine();
                OnLineEnded?.Invoke();
            }
            else
            {
                GoToNextStation();
                OnStationCompleteDepart?.Invoke();
            }
        }
    }


    #region 환승 & 도착 관련 함수

    private int DetermineMaxTransferCount()
    {
        if (GameManager.Instance.GameMode == GameMode.InfiniteMode)
        {
            return INFINITE_TRANSFER_COUNT;
        }

        int stage = GameManager.Instance.CurrentDay;
        return stage == 0 ? 1 : stage + 1;
    }

    // 플레이어가 강제로 환승을 시도할 때 (입석 기능)
    public void ForceTransferByStanding()
    {
        CurTransferCount++;
        GoToNextLine();
    }
    #endregion

    #region 헬퍼 함수

    public float GetCurrentStationStoppingTime()
    {
        return SubwayLines[CurrentLineIdx].stations[CurrentStationIdx].stopTime;
    }

    public float GetRemainTimeToDepartNextStation()
    {
        float total = SubwayLines[CurrentLineIdx].stations[CurrentStationIdx].travelTime + SubwayLines[CurrentLineIdx].stations[CurrentStationIdx].stopTime;
        if (IsStopping)
        {
            return _timeToNextState;
        }
        else
        {
            return _timeToNextState + GetCurrentStationStoppingTime();
        }
    }

    public int GetDreamMapLength()
    // 깨어있던 시간이 TIER1_MAX 이하면 1, TIER2_MAX 이하면 2, TIER3_MAX 이하면 3, 초과면 4를 반환
    {
        if (CurrentLineTime <= AWAKE_TIME_TIER1_MAX && CurrentLineTime >= 0f)
            return 1;
        else if (CurrentLineTime <= AWAKE_TIME_TIER2_MAX && CurrentLineTime > AWAKE_TIME_TIER1_MAX)
            return 2;
        else if (CurrentLineTime <= AWAKE_TIME_TIER3_MAX && CurrentLineTime > AWAKE_TIME_TIER2_MAX)
            return 3;
        else if (CurrentLineTime > AWAKE_TIME_TIER3_MAX)
            return 4;

        else
        {
            Debug.Log("깨어있던 시간이 음수입니다.");
            return 0;
        }
    }
    #endregion
}