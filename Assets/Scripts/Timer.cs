using TMPro;
using UnityEngine;

// SubwayGameManager에서 관리하는 단 하나의 타이머
public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float curTime;
    public float stationTime;
    public bool isStop { get; private set; }

    private int min;
    private int sec;
    private int milSec;

    private void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStop)
        {
            curTime += Time.deltaTime;
            stationTime += Time.deltaTime;
            min = Mathf.FloorToInt(curTime / 60);
            sec = Mathf.FloorToInt(curTime % 60);
            milSec = Mathf.FloorToInt((curTime * 100f) % 100);

            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, milSec);
        }
    }

    void StopTimer()
    {
        isStop = true;
    }

    void StartTimer()
    {
        curTime = 0f;
        isStop = false;
    }

    public void ForceAddTime(float time , float lerpSpeed)
    {
        curTime = Mathf.Lerp(curTime, curTime + time, lerpSpeed); // 필요시 Time.deltaTime 곱하기
    }

    public void ResetStationTimer()
    {
        stationTime = 0f;
    }
}
