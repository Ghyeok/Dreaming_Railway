using System;
using System.Collections.Generic;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    // 씬 컨텍스트 참조
    private SubwaySceneRefs _refs;

    // 시간 랜덤 범위
    private const float minTravelTime = 10f;
    private const float maxTravelTime = 15f;
    private const float minStopTime = 6f;
    private const float maxStopTime = 8f;

    List<SubwayLineData> subwayLines = new List<SubwayLineData>();

    // SubwaySceneBinder가 호출하여 초기화
    public void Init(SubwaySceneRefs refs)
    {
        _refs = refs;
        int maxTransfer = refs.transferManager.MaxTransferCount;

        SubwayFlowManager.Instance.Init();

        if (SubwayFlowManager.Instance.SubwayLines.Count == 0)
        {
            GenerateSubwayLines(maxTransfer);
            SubwayFlowManager.Instance.SetLineData(subwayLines);
        }
        else
        {
            subwayLines = SubwayFlowManager.Instance.SubwayLines;
        }
    }

    public void ResetStationManager()
    {
        int maxTransfer = _refs.transferManager.MaxTransferCount;
        GenerateSubwayLines(maxTransfer);
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