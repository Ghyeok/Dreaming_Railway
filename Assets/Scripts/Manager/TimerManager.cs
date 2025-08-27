using TMPro;
using UnityEngine;

public class TimerManager : SingletonManagers<TimerManager>, IManager
{
    public TextMeshProUGUI timerText;
    public float curTime; // 전체 게임의 시간
    public float lineTime; // 노선 한개를 지나는 시간, 환승하면 0으로 초기화
    public float awakeTime; // 깨어있던 시간
    public float playTime; // 실제 플레이 타임
    public bool isStop { get; private set; }

    private int min;
    private int sec;
    private int milSec;

    public void Init()
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
        if (!SubwayGameManager.Instance.isGameOver && !isStop && GameManager.Instance.gameState != GameManager.GameState.Main && GameManager.Instance.gameState != GameManager.GameState.DaySelect)
        {
            playTime += Time.deltaTime;
            curTime += Time.deltaTime * DreamManager.Instance.dreamTimeSpeed;
            lineTime += Time.deltaTime * DreamManager.Instance.dreamTimeSpeed;

            if (SubwayPlayerManager.Instance.playerState != SubwayPlayerManager.PlayerState.DEEPSLEEP)
            {
                awakeTime += Time.deltaTime;
            }

            min = Mathf.FloorToInt(playTime / 60);
            sec = Mathf.FloorToInt(playTime % 60);
            milSec = Mathf.FloorToInt((playTime * 100f) % 100);

            timerText.text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, milSec);
        }
    }

    public void ResetTimer()
    {
        curTime = 0f;
        lineTime = 0f;
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
