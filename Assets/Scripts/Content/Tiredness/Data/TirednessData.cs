using System;
using UnityEngine;

[System.Serializable]
public class TirednessData
{
    /// <summary>
    /// 현재 피로도
    /// </summary>
    [field: SerializeField] public float CurrentTiredness {  get; private set; } = TirednessConfigData.INITIAL_TIREDNESS;

    /// <summary>
    /// 최대 피로도
    /// </summary>
    [field: SerializeField] public float MaxTiredness { get; private set; } = TirednessConfigData.MAX_TIREDNESS;

    /// <summary>
    /// 피로도 증가가 멈췄는가?
    /// </summary>
    [field: SerializeField] public bool IsPaused { get; private set; } = false;

    /// <summary>
    /// 피로도가 절반 이상 찼는가?
    /// </summary>
    public bool IsTiredHalf => CurrentTiredness >= (MaxTiredness / 2.0f);
    /// <summary>
    /// 피로도가 최대인가?
    /// </summary>
    public bool IsMaxed => CurrentTiredness >= MaxTiredness;

    public event Action<float> OnTiredChanged;
    public event Action OnTiredMaxed;
    /// <summary>
    /// IsTiredHalf가 실제로 바뀔 때만 발행한다. (OnTiredChanged는 매 틱 발행되므로
    /// 애니메이션처럼 전이 시점만 필요한 구독자는 이쪽을 쓴다)
    /// </summary>
    public event Action<bool> OnTiredHalfChanged;

    private bool _wasTiredHalf = false;

    /// <summary>
    /// 값 변경 알림을 한 곳으로 모은다. 모든 변경 메서드는 이걸 거친다.
    /// </summary>
    private void NotifyChanged()
    {
        OnTiredChanged?.Invoke(CurrentTiredness);

        if (_wasTiredHalf == IsTiredHalf) return;

        _wasTiredHalf = IsTiredHalf;
        OnTiredHalfChanged?.Invoke(_wasTiredHalf);
    }

    public void Tick(float deltaTime)
    {
        if (IsPaused || IsMaxed) return;

        CurrentTiredness = Math.Min(CurrentTiredness + deltaTime, MaxTiredness);
        NotifyChanged();

        if (IsMaxed) OnTiredMaxed?.Invoke();
    }

    /// <summary>
    /// 초기값으로 초기화
    /// </summary>
    public void Reset()
    {
        CurrentTiredness = TirednessConfigData.INITIAL_TIREDNESS;
        MaxTiredness = TirednessConfigData.MAX_TIREDNESS;
        IsPaused = false;

        NotifyChanged();
    }

    /// <summary>
    /// 피로도를 특정 값으로 고정하고 증가를 멈춘다. (입석)
    /// </summary>
    public void SetForced(float value)
    {
        IsPaused = true;
        Set(value);
    }

    /// <summary>
    /// 멈춰 있던 피로도 증가를 재개한다.
    /// </summary>
    public void Resume() => IsPaused = false;

    /// <summary>
    /// 피로도를 특정 값으로 강제 설정
    /// </summary>
    public void Set(float value)
    {
        CurrentTiredness = Math.Clamp(value, 0f, MaxTiredness);
        NotifyChanged();
    }

    /// <summary>
    /// 피로도를 줄이는 함수
    /// </summary>
    public void Decrease(float value)
    {
        CurrentTiredness = Math.Clamp(CurrentTiredness - value, 0f, MaxTiredness);
        NotifyChanged();
    }

    /// <summary>
    /// 꿈에 들어갈 때 깨어있던 시간에 따라 피로도 재계산
    /// </summary>
    public void ApplyDreamEnterRecovery(float awakeTime)
    {
        CurrentTiredness = awakeTime <= TirednessConfigData.DREAM_RECOVERY_TIME_THRESHOLD
            ? CurrentTiredness / 2f            // 깨어있는 시간이 100초 이하
            : (CurrentTiredness / 3f) * 2f;    // 깨어있는 시간이 100초 초과

        NotifyChanged();
    }
}
