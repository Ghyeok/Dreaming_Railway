using TMPro;
using UnityEngine;

// SubwayGameManager에서 관리하는 단 하나의 타이머
public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float curTime;
    public bool isStop { get; private set; }

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
            timerText.text = "Time : " + curTime.ToString("F2");
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
}
