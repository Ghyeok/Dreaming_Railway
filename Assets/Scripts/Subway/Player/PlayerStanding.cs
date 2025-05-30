using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStanding : MonoBehaviour
{
    public static event Action OnStandingButtonClicked;

    private void SkipStations() // 입석 버튼이 스킵 버튼으로 변환, 스킵버튼을 누르면 실행되는 함수
    {
        Timer timer = SubwayGameManager.Instance.timer;
        float totalTime = 0;
        float skipSpeed = 1f;

        for (int i = 0; i <= StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].transferIdx; i++)
        {
            totalTime += StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].stations[i].travelTime
                       + StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].stations[i].stopTime;
        }

        timer.ForceAddTime(totalTime, skipSpeed);
        Debug.Log("환승 성공! (입석) ");
        SoundManager.Instance.PlayAudioClip("Standing", Define.Sounds.SFX);
        TransferManager.Instance.curTransferCount++;
        StationManager.Instance.currentLineIdx++;
        SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.STANDING;

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
