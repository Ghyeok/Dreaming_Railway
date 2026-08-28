using UnityEngine;

[System.Serializable]
public class TimerData
{
    // 게임 클리어 / 게임오버 결과 화면에 표시되는 누적 플레이 시간
    [field: SerializeField] public float PlayTime { get; private set; } = 0f;

    // 전역 일시정지. Time.timeScale과는 별개 축으로, 팝업 페이드인을 얼리지 않고 게임플레이만 멈춘다
    [field: SerializeField] public bool IsPaused { get; private set; } = false;

    public void Tick(float deltaTime)
    {
        if (IsPaused) return;

        PlayTime += deltaTime;
    }

    /// <summary>
    /// 새 런 시작 시 초기값으로 초기화
    /// </summary>
    public void Reset()
    {
        PlayTime = 0f;
        IsPaused = false;
    }

    /// <summary>
    /// 게임플레이를 멈춘다. 플레이 타임 누적도 함께 멈춘다.
    /// </summary>
    public void Pause() { IsPaused = true; }

    /// <summary>
    /// 게임플레이를 재개한다.
    /// </summary>
    public void Resume() { IsPaused = false; }
}
