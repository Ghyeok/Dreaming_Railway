using System;
using System.Collections.Generic;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    // 이동/정차 시간 랜덤 범위 (초 단위)
    private const float minTravelTime = 10f;
    private const float maxTravelTime = 15f;
    private const float minStopTime = 6f;
    private const float maxStopTime = 8f;

    // 노선당 생성할 정거장 수 (환승역 인덱스 최대치보다 충분히 크게 설정)
    private const int STATIONS_PER_LINE = 20;

    // 노선 번호에 따른 환승역 인덱스 랜덤 범위
    private const int EARLY_LINE_THRESHOLD = 3;  // 0~3번 노선
    private const int MID_LINE_THRESHOLD = 6;    // 4~6번 노선

    private const int TRANSFER_IDX_EARLY_MIN = 8;
    private const int TRANSFER_IDX_EARLY_MAX = 11;

    private const int TRANSFER_IDX_MID_MIN = 6;
    private const int TRANSFER_IDX_MID_MAX = 9;

    private const int TRANSFER_IDX_LATE_MIN = 4;
    private const int TRANSFER_IDX_LATE_MAX = 7;

    [SerializeField] private List<LineData> subwayLines = new List<LineData>();

    public void Init()
    {
        if (SubwayFlowManager.Instance.SubwayLines.Count == 0)
        {
            GenerateAndApply();
        }
        else
        {
            // 꿈속씬에서 복귀한 경우 — SubwayFlowManager에 이미 데이터 존재
            subwayLines = SubwayFlowManager.Instance.SubwayLines;
        }
    }

    private void GenerateAndApply()
    {
        int lineCount = GameManager.Instance.CurrentDay + 1;
        GenerateSubwayLines(lineCount);
        ChooseStationType();
        SubwayFlowManager.Instance.SetLineData(subwayLines);
    }

    private void GenerateSubwayLines(int lineCount)
    {
        subwayLines.Clear();

        for (int i = 0; i < lineCount; i++)
        {
            LineData newLine = new LineData();
            for (int j = 0; j < STATIONS_PER_LINE; j++)
            {
                newLine.stations.Add(new StationData(minTravelTime, maxTravelTime, minStopTime, maxStopTime));
            }
            subwayLines.Add(newLine);
        }
    }

    private void ChooseStationType()
    {
        bool isNormalMode = GameManager.Instance.GameMode == GameMode.NormalMode;

        for (int i = 0; i < subwayLines.Count; i++)
        {
            bool isDestinationLine = isNormalMode && (i == subwayLines.Count - 1);
            int transferStationIdx;

            if (i <= EARLY_LINE_THRESHOLD) transferStationIdx = UnityEngine.Random.Range(TRANSFER_IDX_EARLY_MIN, TRANSFER_IDX_EARLY_MAX);
            else if (i <= MID_LINE_THRESHOLD) transferStationIdx = UnityEngine.Random.Range(TRANSFER_IDX_MID_MIN, TRANSFER_IDX_MID_MAX);
            else transferStationIdx = UnityEngine.Random.Range(TRANSFER_IDX_LATE_MIN, TRANSFER_IDX_LATE_MAX);

            subwayLines[i].transferIdx = transferStationIdx;
            subwayLines[i].hasDestination = isDestinationLine;

            StationType type = isDestinationLine ? StationType.Destination : StationType.Transfer;
            subwayLines[i].stations[transferStationIdx].stationType = type;
        }
    }
}