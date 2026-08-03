using UnityEngine;

public class TimerManager : SingletonManagers<TimerManager>, IManager
{
    public float playTime;

    public bool IsPaused { get; private set; }

    public void Init()
    {
        playTime = 0f;
        StartTimer();
    }

    private void Update()
    {
        if (IsPaused) return;

        playTime += Time.deltaTime;
    }

    public void StartTimer()
    {
        IsPaused = false;
    }

    public void StopTimer()
    {
        IsPaused = true;
    }
}
