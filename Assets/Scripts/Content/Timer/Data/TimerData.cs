using UnityEngine;

/// <summary>
/// 플레이 타임과 전역 일시정지 상태. 씬과 무관하게 GameDataManager가 소유한다.
/// </summary>
[System.Serializable]
public class TimerData
{
    /// <summary>
    /// 실제 게임 플레이 타임
    /// </summary>
    [field: SerializeField] public float PlayTime { get; private set; } = 0f;

    // 전역 일시정지 — 팝업이 뜬 동안 게임플레이 시스템들의 Tick을 멈추는 용도.
    // Time.timeScale을 건드리지 않으므로 팝업 자신의 페이드인은 계속 재생된다.
    [field: SerializeField] public bool IsPaused { get; private set; } = false;

    public void Tick(float deltaTime)
    {
        if (IsPaused) return;

        PlayTime += deltaTime;
    }

    /// <summary>
    /// 초기값으로 초기화
    /// </summary>
    public void Reset()
    {
        PlayTime = 0f;
        IsPaused = false;
    }

    public void Pause() => IsPaused = true;

    public void Resume() => IsPaused = false;
}
