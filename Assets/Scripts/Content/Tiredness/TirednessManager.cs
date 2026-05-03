using System;
using System.Collections;
using UnityEngine;

public class TirednessManager : MonoBehaviour, ITickable
{
    private const float MAX_TIRED = 100f;
    private const float INITIAL_TIREDNESS = 30f;
    // 꿈에서 깨어난 뒤 피로도 조정 기준 시간 (초)
    private const float DREAM_RECOVERY_TIME_THRESHOLD = 100f;

    public static float MaxTired { get; private set; } = MAX_TIRED;
    public static float CurrentTired { get; private set; } = INITIAL_TIREDNESS;
    public static bool IsTiredHalf => CurrentTired >= (MaxTired / 2.0f); // 애니메이션 트리거, 맵 길이 판단 요소로 사용

    public static event Action<float> OnTiredChange; // 피로도가 변할 때 마다 Invoke, 현재 피로도를 넘겨줌
    public static event Action OnTiredMaxed; // 피로도가 최대일 때 Invoke

    private bool _isPaused = false; // 피로도 증가를 멈출 것인지?
    private bool _isMaxOut = false; // 최대 피로도를 넘었는지?

    public void Init()
    {
        _isPaused = false;
        _isMaxOut = false;

        if (TimerManager.Instance != null)
            TimerManager.Instance.Register(this);
    }

    public void Tick(float deltaTime)
    {
        if (_isPaused || _isMaxOut) return;

        if (CurrentTired < MaxTired)
        {
            CurrentTired += deltaTime;
            OnTiredChange?.Invoke(CurrentTired);
        }
        else
        {
            CurrentTired = MaxTired;
            _isMaxOut = true;
            OnTiredChange?.Invoke(CurrentTired);
            OnTiredMaxed?.Invoke();
        }
    }

    // 초기값으로 초기화
    public void ResetTirednessManager()
    {
        CurrentTired = INITIAL_TIREDNESS;
        MaxTired = MAX_TIRED;
        _isMaxOut = false;
        _isPaused = false;

        OnTiredChange?.Invoke(CurrentTired);
    }

    // 피로도를 강제로 설정하는 함수
    public void SetTirednessForced(float value)
    {
        _isPaused = true;
        CurrentTired = value;
        OnTiredChange?.Invoke(CurrentTired);
    }

    public void SetTirednessAfterDream() // 잠에 들때 피로도 재설정
    {
        float awakeTime = SubwayFlowManager.Instance.CurrentLineTime;

        if (awakeTime <= DREAM_RECOVERY_TIME_THRESHOLD) // 깨어있는 시간이 100초 이하
        {
            CurrentTired /= 2f;
        }
        else // 깨어있는 시간이 100초 초과
        {
            CurrentTired = (CurrentTired / 3f) * 2f;
        }

        _isMaxOut = false;
        OnTiredChange?.Invoke(CurrentTired);
    }

    // 피로도를 줄이는 함수
    public void DecreaseTiredness(float value)
    {
        CurrentTired = Math.Clamp(CurrentTired - value, 0f, MaxTired);
        OnTiredChange?.Invoke(CurrentTired);
    }

    private void OnDestroy()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.Unregister(this);
        }

        OnTiredChange = null;
        OnTiredMaxed = null;
    }
}
