using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubwayData
{

    /// <summary>
    /// 현재 게임의 지하철 노선들
    /// </summary>
    public List<LineData> SubwayLines { get; private set; } = new List<LineData>();

    /// <summary>
    /// 현재 게임에서 노선이 존재하는가?
    /// </summary>
    public bool HasLines => SubwayLines != null && SubwayLines.Count > 0;

    // 진행 / 환승 / 흐름
    /// <summary>
    /// 현재 노선의 인덱스
    /// </summary>
    [field: SerializeField] public int CurrentLineIdx { get; private set; } = 0;
    /// <summary>
    /// 현재 노선의 역의 인덱스
    /// </summary>
    [field: SerializeField] public int CurrentStationIdx { get; private set; } = 0;
    /// <summary>
    /// 현재까지 지나온 역의 수
    /// </summary>
    [field: SerializeField] public int PassedStations { get; private set; } = 0;
    /// <summary>
    /// 현재 노선의 시작으로부터 시간이 얼마나 흘렀는지
    /// </summary>
    [field: SerializeField] public float CurrentLineTime { get; private set; } = 0;
    /// <summary>
    /// 현재 지하철이 정차 구간에 있는가?
    /// </summary>
    [field: SerializeField] public bool IsSubwayStopping { get; private set; } = false;
    /// <summary>
    /// 현재 환승 횟수
    /// </summary>
    [field: SerializeField] public int CurTransferCount { get; private set; } = 0;
    /// <summary>
    /// 최대 환승 횟수
    /// </summary>
    [field: SerializeField] public int MaxTransferCount { get; private set; } = 0; // 최대 환승 횟수 = (노선 - 1)
    /// <summary>
    /// 노선 시간의 흐름 속도 -> 지하철에서는 1.0, 꿈 속에서는 3.0 ~ 4.0
    /// </summary>
    [field: SerializeField] public float FlowSpeed { get; private set; } = SubwayConfigData.NORMAL_FLOW_SPEED;
    /// <summary>
    /// 운행 <-> 정차 구간 누적 시간 합
    /// </summary>
    [field: SerializeField] public float TimeToNextState { get; private set; } = 0;
    /// <summary>
    /// 이번 Day를 클리어 했는지?
    /// </summary>
    [field: SerializeField] public bool IsRunFinished { get; private set; }

    // 뺨 / 입석 / 게임오버
    /// <summary>
    /// 뺨 때리기 쿨타임
    /// </summary>
    [field: SerializeField] public float SlapCoolTime { get; private set; } = SubwayConfigData.INITIAL_SLAP_COOP_TIME;
    /// <summary>
    /// 뺨 때린 횟수
    /// </summary>
    [field: SerializeField] public int SlapNum { get; private set; } = 0;
    /// <summary>
    /// 뺨 때리면 줄어드는 피로도 수치
    /// </summary>
    [field: SerializeField] public float TiredDecreaseBySlap { get; private set; } = SubwayConfigData.INITIAL_TIREDNESS_DECREASE_BY_SLAP;
    /// <summary>
    /// 뺨 때리기가 쿨타임인가?
    /// </summary>
    [field: SerializeField] public bool IsSlapCoolTime { get; private set; } = false;
    /// <summary>
    /// 현재 남은 뺨 때리기 쿨타임
    /// </summary>
    [field: SerializeField] public float CurrentSlapCooldown { get; private set; } = 0f;
    /// <summary>
    /// 입석 횟수 -> 입석 쿨타임 측정용
    /// </summary>
    [field: SerializeField] public int StandingCount { get; private set; } = 0;
    /// <summary>
    /// 입석이 쿨타임인가?
    /// </summary>
    [field: SerializeField] public bool IsStandingCoolDown { get; private set; } = false;
    /// <summary>
    /// 지하철 내에서 게임오버 되었는가? -> 꿈 속에서 탈출에는 성공했지만 이미 내릴 역을 지나친 상태
    /// </summary>
    [field: SerializeField] public bool IsGameOverInSubway { get; private set; } = false;

    // 이벤트
    public event Action<float> OnTimeUpdated;
    public event Action<bool> OnStationEnterStopInterval;
    public event Action OnStationCompleteDepart;
    public event Action OnLineEnded;
    public event Action OnDayCleared;
    public event Action OnSubwayGameOver;
    public event Action OnStandingCooldownChanged;

    /// <summary>
    /// 현재 노선 데이터
    /// </summary>
    public LineData CurrentLineData => SubwayLines[CurrentLineIdx];
    /// <summary>
    /// 현재 역 데이터
    /// </summary>
    public StationData CurrentStationData => CurrentLineData.stations[CurrentStationIdx];

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
    /// 새 런을 위해 노선을 비운다.
    /// StationSystem.Init()이 HasLines로 재생성 여부를 판단하므로, 비우면 재생성 →
    /// BeginRun()으로 진행 상태까지 초기화되는 기존 경로를 그대로 탄다.
    /// </summary>
    public void Reset() => SubwayLines = new List<LineData>();

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
        OnStandingCooldownChanged?.Invoke();

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

            if (IsRunFinished) break;
        }
    }

    /// <summary>
    /// 뺨 때리기 쿨다운
    /// </summary>
    /// <param name="deltaTime"></param>
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

    /// <summary>
    /// 운행 <-> 정차 구간 변경
    /// </summary>
    private void AdvanceState()
    {
        bool isFinalStation = CurrentStationIdx == CurrentLineData.transferIdx
                      && CurTransferCount == MaxTransferCount - 1;

        if (!IsSubwayStopping) // 운행 구간이었다면 정차 구간 진입
        {
            IsSubwayStopping = true;
            TimeToNextState += CurrentLineData.stations[CurrentStationIdx].stopTime;

            OnStationEnterStopInterval?.Invoke(isFinalStation); // 노선의 마지막 역이었다면 뒷 배경을 역 플랫폼 배경으로
        }
        else // 정차 구간이면
        {
            if (isFinalStation) // 노선의 마지막 역
            {
                GoToNextLine();
                AddStandingCount();
                OnLineEnded?.Invoke();
            }
            else // 마지막 역이 아니면
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

    /// <summary>
    /// 동일 노선의 다음 역으로 이동
    /// </summary>
    private void GoToNextStation()
    {
        IsSubwayStopping = false;
        CurrentStationIdx++;
        PassedStations++;
        TimeToNextState += CurrentStationData.travelTime;
    }

    #endregion

    #region 상태 변경

    /// <summary>
    /// 노선 시간 흐름 속도 변경
    /// </summary>
    /// <param name="isDream"></param>
    public void SetFlowSpeed(bool isDream)
    {
        FlowSpeed = isDream?
              UnityEngine.Random.Range(SubwayConfigData.DREAM_MIN_FLOW_SPEED, SubwayConfigData.DREAM_MAX_FLOW_SPEED)
            : SubwayConfigData.NORMAL_FLOW_SPEED;
    }

    /// <summary>
    /// 플레이어가 강제로 환승 진행 -> 입석 로직
    /// </summary>
    public void ForceTransferByStanding() => GoToNextLine();

    /// <summary>
    /// 뺨 때림 -> 뺨 때리기 쿨다운 시작
    /// </summary>
    public void StartSlapCooldown()
    {
        SlapNum++;
        IsSlapCoolTime = true;
        CurrentSlapCooldown = SlapCoolTime;
    }

    /// <summary>
    /// 입석 성공 -> 입석 쿨타임 시작
    /// </summary>
    public void StartStandingCooldown()
    {
        IsStandingCoolDown = true;
        StandingCount = 0;
        OnStandingCooldownChanged?.Invoke();
    }

    /// <summary>
    /// 노선이 하나 끝날 때마다 입석 쿨다운을 한 칸 진행시킨다.
    /// 입석 스킵(ForceTransferByStanding)은 이 경로를 타지 않는다 — 쿨다운이 그 자리에서 새면 안 되기 때문.
    /// </summary>
    private void AddStandingCount()
    {
        if (!IsStandingCoolDown) return;

        StandingCount++;
        if (StandingCount >= SubwayConfigData.STANDING_COOLDOWN_LINES)
        {
            IsStandingCoolDown = false;
            StandingCount = 0;
        }

        OnStandingCooldownChanged?.Invoke();
    }

    /// <summary>
    /// 꿈에서 탈출했지만 내릴 역을 놓침 -> 게임 오버 상태로 전환
    /// </summary>
    public void SetGameOver()
    {
        if (IsGameOverInSubway) return;

        IsGameOverInSubway = true;
        OnSubwayGameOver?.Invoke();
    }

    #endregion

    #region 헬퍼 함수

    /// <summary>
    /// 현재 노선의 역의 정차구간 길이 반환
    /// </summary>
    /// <returns></returns>
    public float GetCurrentStationStoppingTime() => CurrentStationData.stopTime;

    /// <summary>
    /// 다음 역까지 남은 시간 반환
    /// </summary>
    /// <returns></returns>
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
