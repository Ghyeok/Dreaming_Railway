using TMPro;
using UnityEngine;

// SubwayGameManager에서 관리하는 단 하나의 타이머
public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float curTime; // 전체 게임의 시간
    public float stationTime; // 역 한개를 지나는 시간
    public float awakeTime; // 깨어있던 시간
    public bool isStop { get; private set; }

    private int min;
    private int sec;
    private int milSec;

    private void Awake()
    {

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

            if (SubwayPlayerManager.Instance.playerState != SubwayPlayerManager.PlayerState.DEEPSLEEP)
            {
                stationTime += Time.deltaTime;
                awakeTime += Time.deltaTime;
            }

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
        isStop = false;
    }

    public void ForceAddTime(float time , float lerpSpeed)
    {
        curTime = Mathf.Lerp(curTime, curTime + time, lerpSpeed); // 필요시 Time.deltaTime 곱하기
    }
}
