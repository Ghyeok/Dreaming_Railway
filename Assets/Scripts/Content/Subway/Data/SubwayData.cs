using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubwayData
{
    // 노선 데이터
    public List<LineData> SubwayLines { get; private set; } = new List<LineData>();
    public bool HasLines => SubwayLines != null && SubwayLines.Count > 0;

    // 진행 / 환승 / 흐름
    [field: SerializeField] public int CurrentLineIdx { get; private set; } = 0; // 현재 노선 인덱스
    [field: SerializeField] public int CurrentStationIdx { get; private set; } = 0; // 현재 역 인덱스
    [field: SerializeField] public int PassedStations { get; private set; } = 0; // 지나온 역 수
    [field: SerializeField] public float CurrentLineTime { get; private set; } = 0; // 현재 노선 시간
    [field: SerializeField] public bool IsSubwayStopping { get; private set; } = false; // 정차 중인가?
    [field: SerializeField] public int CurTransferCount { get; private set; } = 0; // 현재 환승 횟수
    [field: SerializeField] public int MaxTransferCount { get; private set; } = 0; // 최대 환승 횟수 = (노선 - 1)
    [field: SerializeField] public float FlowSpeed { get; private set; } = SubwayConfigData.NORMAL_FLOW_SPEED; // 시간 흐름 속도
    [field: SerializeField] public float TimeToNextState { get; private set; } = 0; // 운행 상태 <-> 정차 상태 까지 걸리는 시간
    [field: SerializeField] public bool IsRunFinished { get; private set; } // Day 클리어 후 틱 정지

    // 뺨 / 입석 / 게임오버
    [field: SerializeField] public float SlapCoolTime { get; private set; } = SubwayConfigData.INITIAL_SLAP_COOP_TIME; // 뺨 때리기 쿨타임
    [field: SerializeField] public int SlapNum { get; private set; } = 0; // 뺨 때린 횟수
    [field: SerializeField] public float TiredDecreaseBySlap { get; private set; } = SubwayConfigData.INITIAL_TIREDNESS_DECREASE_BY_SLAP; // 뺨 때리면 줄어드는 피로도 수치
    [field: SerializeField] public bool IsSlapCoolTime { get; private set; } = false; // 뺨 때리기 쿨타임인가?
    [field: SerializeField] public float CurrentSlapCooldown { get; private set; } = 0f; // 현재 남은 뺨 때리기 쿨타임
    [field: SerializeField] public int StandingCount { get; private set; } = 0; // 입석 횟수
    [field: SerializeField] public bool IsStandingCoolDown { get; private set; } = false; // 입석 쿨타임인가?
    [field: SerializeField] public bool IsGameOverInSubway { get; private set; } = false; // 지하철 내에서 게임오버 되었는가?

    // 이벤트
    public event Action<float> OnTimeUpdated; // 시간 갱신
    public event Action<bool> OnStationEnterStopInterval;
    public event Action OnStationCompleteDepart;
    public event Action OnLineEnded;
    public event Action OnDayCleared;
    public event Action OnSubwayGameOver;

    public LineData CurrentLine => SubwayLines[CurrentLineIdx]; // 현재 노선
    public StationData CurrentStation => CurrentLine.stations[CurrentStationIdx]; // 현재 역

    #region 초기화 / 리셋

    /// <summary>
    /// 새 런 시작 — 노선을 주입하고 진행 상태를 초기화한다.
    /// </summary>
    public void BeginRun(List<LineData> lines, int maxTransferCount)
    {
        SubwayLines = lines;
        ResetProgress(maxTransferCount);
    }

    /// <summary>
    /// 노선 진행 상태만 초기값으로 되돌린다. (뺨/입석 데이터는 건드리지 않음)
    /// </summary>
    public void ResetProgress(int maxTransferCount)
    {
        CurrentLineIdx = 0;
        CurrentStationIdx = 0;
        PassedStations = 0;
        CurrentLineTime = 0f;
        IsSubwayStopping = false;

        CurTransferCount = 0;
        MaxTransferCount = maxTransferCount;
        IsRunFinished = false;

        FlowSpeed = SubwayConfigData.NORMAL_FLOW_SPEED;
        TimeToNextState = 0f;

        if (HasLines && SubwayLines[0].stations.Count > 0)
        {
            TimeToNextState = SubwayLines[0].stations[0].travelTime;
        }
    }

    /// <summary>
    /// 뺨/입석/게임오버 데이터를 초기화한다. 지하철 씬에 진입할 때마다 호출.
    /// </summary>
    public void ResetPlayerSession(bool isInfiniteMode)
    {
        SlapNum = 0;
        IsSlapCoolTime = false;
        SlapCoolTime = SubwayConfigData.INITIAL_SLAP_COOP_TIME;
        CurrentSlapCooldown = 0f;
        TiredDecreaseBySlap = isInfiniteMode
            ? SubwayConfigData.INFINITE_TIREDNESS_DECREASE_BY_SLAP
            : SubwayConfigData.INITIAL_TIREDNESS_DECREASE_BY_SLAP;

        StandingCount = 0;
        IsStandingCoolDown = false;

        IsGameOverInSubway = false;
    }

    #endregion

    #region 틱

    public void Tick(float deltaTime)
    {
        if (!HasLines || IsRunFinished) return;

        float scaled = deltaTime * FlowSpeed;

        CurrentLineTime += scaled;
        OnTimeUpdated?.Invoke(CurrentLineTime);

        TimeToNextState -= scaled;

        while (TimeToNextState <= 0f) // 0보다 작으면 구간이 바뀌어야 함
        {
            AdvanceState();

            // Day 클리어 시 CurrentLineIdx가 범위를 벗어나므로 반드시 탈출해야 한다
            if (IsRunFinished) break;
        }
    }

    public void TickSlapCooldown(float deltaTime)
    {
        if (!IsSlapCoolTime) return;

        CurrentSlapCooldown -= deltaTime;
        if (CurrentSlapCooldown <= 0f)
        {
            IsSlapCoolTime = false;
            CurrentSlapCooldown = SlapCoolTime;
        }
    }

    private void AdvanceState()
    {
        if (!IsSubwayStopping) // 정차 구간이 아니면 정차 구간 진입
        {
            IsSubwayStopping = true;
            TimeToNextState += CurrentLine.stations[CurrentStationIdx].stopTime;

            bool isFinalStation = CurrentStationIdx == CurrentLine.transferIdx
                                  && CurTransferCount == MaxTransferCount - 1;
            OnStationEnterStopInterval?.Invoke(isFinalStation);
        }
        else // 정차 구간이면
        {
            if (CurrentStationIdx == CurrentLine.transferIdx) // 노선의 마지막 역
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

    /// <summary>
    /// 다음 노선으로 이동, 환승이냐 도착이냐에 따라 로직 분기
    /// </summary>
    private void GoToNextLine()
    {
        CurTransferCount++;
        CurrentLineIdx++;

        if (CurTransferCount >= MaxTransferCount) // 노선의 끝, Day 종료
        {
            IsRunFinished = true;
            OnDayCleared?.Invoke();
            return;
        }

        CurrentStationIdx = 0;
        CurrentLineTime = 0f;
        IsSubwayStopping = false;

        if (CurrentLineIdx < SubwayLines.Count && SubwayLines[CurrentLineIdx].stations.Count > 0)
        {
            TimeToNextState = SubwayLines[CurrentLineIdx].stations[0].travelTime;
        }
    }

    private void GoToNextStation()
    {
        IsSubwayStopping = false;
        CurrentStationIdx++;
        PassedStations++;
        TimeToNextState += CurrentStation.travelTime;
    }

    #endregion

    #region 상태 변경

    public void SetFlowSpeed(bool isDream)
    {
        FlowSpeed = isDream
            ? UnityEngine.Random.Range(SubwayConfigData.DREAM_MIN_FLOW_SPEED, SubwayConfigData.DREAM_MAX_FLOW_SPEED)
            : SubwayConfigData.NORMAL_FLOW_SPEED;
    }

    // 플레이어가 강제로 환승을 시도할 때 (입석 기능)
    public void ForceTransferByStanding() => GoToNextLine();

    public void StartSlapCooldown()
    {
        SlapNum++;
        IsSlapCoolTime = true;
        CurrentSlapCooldown = SlapCoolTime;
    }

    public void StartStandingCooldown()
    {
        IsStandingCoolDown = true;
        StandingCount = 0;
    }

    public void AddStandingCount()
    {
        if (!IsStandingCoolDown) return;

        StandingCount++;
        if (StandingCount >= SubwayConfigData.STANDING_COOLDOWN_LINES)
        {
            IsStandingCoolDown = false;
            StandingCount = 0;
        }
    }

    public void SetGameOver()
    {
        if (IsGameOverInSubway) return;

        IsGameOverInSubway = true;
        OnSubwayGameOver?.Invoke();
    }

    #endregion

    #region 헬퍼 함수

    public float GetCurrentStationStoppingTime() => CurrentStation.stopTime;

    public float GetRemainTimeToDepartNextStation()
    {
        if (IsSubwayStopping) return TimeToNextState;

        return TimeToNextState + GetCurrentStationStoppingTime();
    }

    /// <summary>
    /// 깨어있던 시간이 TIER1_MAX 이하면 1, TIER2_MAX 이하면 2, TIER3_MAX 이하면 3, 초과면 4를 반환
    /// </summary>
    public int GetDreamMapLength()
    {
        if (CurrentLineTime < 0f)
        {
            Debug.Log("깨어있던 시간이 음수입니다.");
            return 0;
        }

        if (CurrentLineTime <= SubwayConfigData.AWAKE_TIME_TIER1_MAX) return 1;
        if (CurrentLineTime <= SubwayConfigData.AWAKE_TIME_TIER2_MAX) return 2;
        if (CurrentLineTime <= SubwayConfigData.AWAKE_TIME_TIER3_MAX) return 3;

        return 4;
    }

    #endregion
}
