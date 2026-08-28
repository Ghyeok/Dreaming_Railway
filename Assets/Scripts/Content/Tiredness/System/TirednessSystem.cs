using UnityEngine;

public class TirednessSystem : MonoBehaviour
{
    private TirednessData _data; // 실제 데이터
    private TimerData _timer;
    private bool _isPaused = false; // 피로도 증가를 멈출 것인지?

    public void Init()
    {
        _data = GameDataManager.Instance.Tiredness;
        _timer = GameDataManager.Instance.Timer;
        _isPaused = false;
    }

    private void Update()
    {
        if (_data == null || _isPaused) return;
        if (_timer == null || _timer.IsPaused) return;

        _data.Tick(Time.deltaTime);
    }

    // 초기값으로 초기화
    public void ResetTiredness()
    {
        _isPaused = false;
        _data.Reset();
    }

    // 피로도를 강제로 설정하고 증가를 멈추는 함수
    public void SetTirednessForced(float value)
    {
        _isPaused = true;
        _data.Set(value);
    }

    public void SetTirednessOnDreamEnter() // 잠에 들때 피로도 재설정
    {
        _data.ApplyDreamEnterRecovery(GameDataManager.Instance.Subway.CurrentLineTime);
    }

    // 피로도를 줄이는 함수
    public void DecreaseTiredness(float value) => _data.Decrease(value);
}
