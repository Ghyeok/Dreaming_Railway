using System.Collections.Generic;
using UnityEngine;

public class StationSystem : MonoBehaviour
{
    // 이동/정차 시간 랜덤 범위 (초 단위)
    private const float MIN_TRAVEL_TIME = 10f;
    private const float MAX_TRAVEL_TIME = 15f;
    private const float MIN_STOP_TIME = 6f;
    private const float MAX_STOP_TIME = 8f;

    // 노선당 생성할 정거장 수 (환승역 인덱스 최대치보다 충분히 크게 설정)
    private const int STATIONS_PER_LINE = 20;

    // 무한 모드 (노선 수 / 최대 환승 횟수 무제한)
    private const int INFINITE_LINE_COUNT = 999;
    private const int INFINITE_TRANSFER_COUNT = 999;

    // 노선 번호에 따른 환승역 인덱스 랜덤 범위
    private const int EARLY_LINE_THRESHOLD = 3;  // 0 ~ 3번 노선
    private const int MID_LINE_THRESHOLD = 6;    // 4 ~ 6번 노선

    private const int TRANSFER_IDX_EARLY_MIN = 8;
    private const int TRANSFER_IDX_EARLY_MAX = 11;

    private const int TRANSFER_IDX_MID_MIN = 6;
    private const int TRANSFER_IDX_MID_MAX = 9;

    private const int TRANSFER_IDX_LATE_MIN = 4;
    private const int TRANSFER_IDX_LATE_MAX = 7;

    private SubwayData _subway;
    private bool _isNormalMode;

    public void Init()
    {
        _subway = GameDataManager.Instance.Subway;
        // 필드 이니셜라이저로 두면 Awake 이전(씬 역직렬화 중)에 평가되므로 여기서 읽는다
        _isNormalMode = GameManager.Instance.GameMode == GameMode.NormalMode;

        // 꿈속씬에서 복귀한 경우 — 이미 노선 데이터가 있으므로 재생성/리셋하지 않는다
        if (_subway.HasLines) return;

        List<LineData> lines = GenerateSubwayLines(GetLineCount());
        InitAllLines(lines);

        _subway.BeginRun(lines, DetermineMaxTransferCount());
    }

    private int GetLineCount() => _isNormalMode ? GameManager.Instance.CurrentDay + 1 : INFINITE_LINE_COUNT;

    /// <summary>
    /// 현재 Day에 맞는 최대 환승 횟수 결정하기
    /// </summary>
    private int DetermineMaxTransferCount()
    {
        if (GameManager.Instance.GameMode == GameMode.InfiniteMode)
        {
            return INFINITE_TRANSFER_COUNT;
        }

        int stage = GameManager.Instance.CurrentDay;
        return stage == 0 ? 1 : stage + 1;
    }

    /// <summary>
    /// 노선의 기본 뼈대를 생성해주는 함수
    /// </summary>
    private List<LineData> GenerateSubwayLines(int lineCount)
    {
        List<LineData> lines = new List<LineData>(lineCount);

        for (int i = 0; i < lineCount; i++)
        {
            LineData newLine = new LineData();
            for (int j = 0; j < STATIONS_PER_LINE; j++)
            {
                newLine.stations.Add(new StationData(MIN_TRAVEL_TIME, MAX_TRAVEL_TIME, MIN_STOP_TIME, MAX_STOP_TIME));
            }
            lines.Add(newLine);
        }

        return lines;
    }

    /// <summary>
    /// 현재 Day의 모든 노선들의 초기값들을 세팅해주는 함수
    /// </summary>
    private void InitAllLines(List<LineData> lines)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            bool isDestinationLine = _isNormalMode && (i == lines.Count - 1);
            int transferStationIdx;

            if (i <= EARLY_LINE_THRESHOLD) 
                transferStationIdx = UnityEngine.Random.Range(TRANSFER_IDX_EARLY_MIN, TRANSFER_IDX_EARLY_MAX);
            else if (i > EARLY_LINE_THRESHOLD && i <= MID_LINE_THRESHOLD) 
                transferStationIdx = UnityEngine.Random.Range(TRANSFER_IDX_MID_MIN, TRANSFER_IDX_MID_MAX);
            else
                transferStationIdx = UnityEngine.Random.Range(TRANSFER_IDX_LATE_MIN, TRANSFER_IDX_LATE_MAX);

            // 현재 노선의 환승역과 도착역 표시
            lines[i].transferIdx = transferStationIdx;
            lines[i].hasDestination = isDestinationLine;

            StationType type = isDestinationLine ? StationType.Destination : StationType.Transfer;
            lines[i].stations[transferStationIdx].stationType = type;
        }
    }
}
