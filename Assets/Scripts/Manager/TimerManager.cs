using TMPro;
using UnityEngine;

public class TimerManager : SingletonManagers<TimerManager>
{
    public TextMeshProUGUI timerText;
    public float curTime; // 전체 게임의 시간
    public float stationTime; // 역 한개를 지나는 시간, 환승하면 0으로 초기화
    public float awakeTime; // 깨어있던 시간
    public float playTime; // 실제 플레이 타임
    public bool isStop { get; private set; }

    private int min;
    private int sec;
    private int milSec;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        FlowTime();
    }

    private void FlowTime()
    {
        if (!isStop && GameManager.Instance.gameState != GameManager.GameState.Main && GameManager.Instance.gameState != GameManager.GameState.DaySelect)
        {
            playTime += Time.deltaTime;
            curTime += Time.deltaTime * DreamManager.Instance.dreamTimeSpeed;
            stationTime += Time.deltaTime * DreamManager.Instance.dreamTimeSpeed;

            if (SubwayPlayerManager.Instance.playerState != SubwayPlayerManager.PlayerState.DEEPSLEEP)
            {
                awakeTime += Time.deltaTime;
            }

            min = Mathf.FloorToInt(curTime / 60);
            sec = Mathf.FloorToInt(curTime % 60);
            milSec = Mathf.FloorToInt((curTime * 100f) % 100);

            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, milSec);
        }
        else if(GameManager.Instance.gameState == GameManager.GameState.Main || GameManager.Instance.gameState == GameManager.GameState.DaySelect)
        {
            ResetTimer();
        }
    }

    public void ResetTimer()
    {
        curTime = 0f;
        stationTime = 0f;
        awakeTime = 0f;
        playTime = 0f;
    }

    public void StopTimer()
    {
        isStop = true;
    }

    public void StartTimer()
    {
        isStop = false;
    }

    public void ForceAddTime(float time, float lerpSpeed)
    {
        curTime = Mathf.Lerp(curTime, curTime + time, lerpSpeed); // 필요시 Time.deltaTime 곱하기
    }
}
