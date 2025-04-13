using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public float curTime;
    public bool isStop;

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
            timer.text = "Time : " + curTime.ToString("F2");
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
}
