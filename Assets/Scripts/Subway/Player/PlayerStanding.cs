using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStanding : MonoBehaviour
{
    public static event Action OnStandingButtonClicked;

    private void SkipStations() // 현재 위치에서 환승역까지의 시간을 더하는 함수
    {
        int lindIdx = StationManager.Instance.currentLineIdx;
        int curIdx = StationManager.Instance.currentStationIdx;
        int transferIdx = StationManager.Instance.subwayLines[lindIdx].transferIdx;

        float skipTime = 0f;
        float curTime = SubwayGameManager.Instance.timer.curTime;
        float accumulated = 0f;

        for (int i = 0; i < transferIdx; i++)
        {
            var station = StationManager.Instance.subwayLines[lindIdx].stations[i];
            float travel = station.travelTime;
            float stop = station.stopTime;

            float start = accumulated;
            float end = accumulated + travel + stop;

            if (i < curIdx)
            {
                // 지나간 역은 무시
            }
            else if (i == curIdx)
            {
                // 현재역은 남은 시간만 계산
                float remain = Mathf.Max(0f, (start + travel + stop) - curTime);
                skipTime += remain;
            }
            else if (i > curIdx)
            {
                skipTime += travel + stop;
            }
            accumulated += travel + stop;
        }

        SubwayGameManager.Instance.timer.curTime += skipTime;

        TransferManager.Instance.ForceTransferByStanding();
    }

    public static void TriggerStanding()
    {
        OnStandingButtonClicked?.Invoke();
    }

    private void OnEnable()
    {
        OnStandingButtonClicked += SkipStations;
    }

    private void OnDisable()
    {
        OnStandingButtonClicked -= SkipStations;
    }
}
