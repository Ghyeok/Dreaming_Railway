using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStanding : MonoBehaviour
{
    public static event Action OnStandingButtonClicked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SkipStations()
    {
        Timer timer = SubwayGameManager.Instance.timer;
        float totalTime = 0;
        float skipSpeed = 1f;

        for (int i = 0; i <= StationManager.Instance.transferIdx; i++)
        {
            totalTime += StationManager.Instance.stationDatas[i].travelTime + StationManager.Instance.stationDatas[i].stopTime;
        }

        timer.ForceAddTime(totalTime, skipSpeed);
        Debug.Log("환승 성공! (입석) ");
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
