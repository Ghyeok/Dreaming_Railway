using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStanding : MonoBehaviour
{
    public static event Action OnStandingButtonClicked;

    void SkipStations()
    {
        Timer timer = SubwayGameManager.Instance.timer;
        float totalTime = 0;
        float skipSpeed = 1f;

        for (int i = 0; i <= StationManager.Instance.transferStationIdx; i++)
        {
            totalTime += StationManager.Instance.stationDatas[i].travelTime + StationManager.Instance.stationDatas[i].stopTime;
        }

        timer.ForceAddTime(totalTime, skipSpeed);
        Debug.Log("환승 성공! (입석) ");
        SoundManager.Instance.PlayAudioClip("Standing", Define.Sounds.SFX);
        TransferManager.Instance.curTransferCount++;
        StationManager.Instance.GenerateStations();
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
