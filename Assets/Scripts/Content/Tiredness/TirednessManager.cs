using System;
using System.Collections;
using UnityEngine;

public class TirednessManager : MonoBehaviour, ITickable
{
    private SubwaySceneRefs _refs;
    [field: SerializeField] public float MaxTired { get; private set; } = 100f;
    [field: SerializeField] public float CurrentTired { get; private set; } = 30f;
    public bool IsTiredHalf => CurrentTired >= (MaxTired / 2.0f); // 애니메이션 트리거, 맵 길이 판단 요소로 사용

    public event Action<float> OnTiredChange; // 피로도가 변할 때 마다 Invoke, 현재 피로도를 넘겨줌
    public event Action OnTiredMaxed; // 피로도가 최대일 때 Invoke

    private bool _isPaused = false; // 피로도 증가를 멈출 것인지?
    private bool _isMaxOut = false; // 최대 피로도를 넘었는지?

    public void Init(SubwaySceneRefs refs)
    {
        _refs = refs;

        CurrentTired = 30f;
        MaxTired = 100f;
        _isMaxOut = false;

        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.Register(this);
        }
        else
        {
            Debug.Log("타이머 매니저 없음");
        }
    }

    public void Tick(float deltaTime)
    {
        if (_isPaused || _isMaxOut) return;

        CurrentTired += deltaTime;
        OnTiredChange?.Invoke(CurrentTired);

        if (CurrentTired >= MaxTired)
        {
            CurrentTired = MaxTired;
            _isMaxOut = true;
            OnTiredMaxed?.Invoke();
        }
    }

    public void ResetTiredManager()
    {
        CurrentTired = 30f;
        _isMaxOut = false;
        _isPaused = false;

        OnTiredChange?.Invoke(CurrentTired);
    }

    public void SetTiredAfterDream() // 잠에 들때 피로도 재설정
    {
        float awakeTime = _refs.stationManager.CurrentLineTime;

        if (awakeTime <= 100f)
        {
            CurrentTired /= 2f;
        }
        else if (awakeTime > 100f)
        {
            CurrentTired = (CurrentTired / 2f) * 3f;
        }

        _isMaxOut = false;
        OnTiredChange?.Invoke(CurrentTired);
    }

    private void OnDestroy()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.Unregister(this);
        }
    }
}
