using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerManager : SingletonManagers<TimerManager>, IManager
{
    private List<ITickable> _tickable = new List<ITickable>();
    public float playTime;

    public bool IsPaused {  get; private set; }

    public void Init()
    {
        StartTimer();
    }

    private void Update()
    {
        if (IsPaused) return;

        float dt = Time.deltaTime;

        Debug.Log($"현재 등록된 객체: {_tickable.Count}개");
        // 역순으로 순회하여 리스트 크기가 중간에 변해도 인덱스는 변하지 않게 함
        for (int i = _tickable.Count - 1; i >= 0; i--)
        {
            _tickable[i].Tick(dt);
        }
    }

    /// <summary>
    /// 매 프레임 실행될 객체를 등록
    /// </summary>
    public void Register(ITickable tickable)
    {
        if (!_tickable.Contains(tickable))
        {
            _tickable.Add(tickable);
        }
    }

    /// <summary>
    /// 매 프레임 실행될 객체를 삭제
    /// </summary>
    public void Unregister(ITickable tickable)
    {
        if (_tickable.Contains(tickable))
        {
            _tickable.Remove(tickable);
        }
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
