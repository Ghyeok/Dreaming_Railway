using System;

public class TirednessData
{
    public TirednessData(float initial, float max)
    {
        CurrentTiredness = initial;
        MaxTiredness = max;
    }

    private const float MAX_TIREDNESS = 100f;
    private const float INITIAL_TIREDNESS = 30f;
    private const float DREAM_RECOVERY_TIME_THRESHOLD = 100f; // 꿈 진입 시 피로도 조정 기준 시간 (초)

    public float CurrentTiredness {  get; private set; } = INITIAL_TIREDNESS;

    public float MaxTiredness { get; private set; } = MAX_TIREDNESS;

    public bool IsTiredHalf => CurrentTiredness >= (MaxTiredness / 2.0f);
    public bool IsMaxed => CurrentTiredness >= MaxTiredness;

    public event Action<float> OnTiredChange; // 피로도가 변할 때 마다 Invoke, 현재 피로도를 넘겨줌
    public event Action OnTiredMaxed; // 피로도가 최대에 도달했을 때 Invoke

    public void Tick(float deltaTime)
    {
        if (IsMaxed) return;

        CurrentTiredness = Math.Min(CurrentTiredness + deltaTime, MaxTiredness);
        OnTiredChange?.Invoke(CurrentTiredness);

        if (IsMaxed) OnTiredMaxed?.Invoke();
    }

    /// <summary>
    /// 초기값으로 초기화
    /// </summary>
    public void Reset()
    {
        CurrentTiredness = INITIAL_TIREDNESS;
        MaxTiredness = MAX_TIREDNESS;

        OnTiredChange?.Invoke(CurrentTiredness);
    }

    /// <summary>
    /// 피로도를 특정 값으로 강제 설정
    /// </summary>
    public void Set(float value)
    {
        CurrentTiredness = Math.Clamp(value, 0f, MaxTiredness);
        OnTiredChange?.Invoke(CurrentTiredness);
    }

    /// <summary>
    /// 피로도를 줄이는 함수
    /// </summary>
    public void Decrease(float value)
    {
        CurrentTiredness = Math.Clamp(CurrentTiredness - value, 0f, MaxTiredness);
        OnTiredChange?.Invoke(CurrentTiredness);
    }

    /// <summary>
    /// 꿈에 들어갈 때 깨어있던 시간에 따라 피로도 재계산
    /// </summary>
    public void ApplyDreamEnterRecovery(float awakeTime)
    {
        CurrentTiredness = awakeTime <= DREAM_RECOVERY_TIME_THRESHOLD
            ? CurrentTiredness / 2f            // 깨어있는 시간이 100초 이하
            : (CurrentTiredness / 3f) * 2f;    // 깨어있는 시간이 100초 초과

        OnTiredChange?.Invoke(CurrentTiredness);
    }
}
