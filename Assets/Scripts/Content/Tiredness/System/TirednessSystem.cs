using UnityEngine;

/// <summary>
/// 피로도 데이터를 매 프레임 구동하는 시스템. 상태는 갖지 않는다.
/// 값 변경이 필요한 쪽은 이 클래스를 거치지 말고 TirednessData를 직접 호출한다.
/// </summary>
public class TirednessSystem : MonoBehaviour
{
    private TirednessData _data; // 실제 데이터
    private TimerData _timer;

    public TirednessData Data => _data;

    public void Init()
    {
        _data = GameDataManager.Instance.Tiredness;
        _timer = GameDataManager.Instance.Timer;

        // 입석으로 멈춰 있던 증가를 지하철 씬 재진입 시 재개한다
        _data.Resume();
    }

    private void Update()
    {
        if (_data == null) return;
        if (_timer == null || _timer.IsPaused) return;

        _data.Tick(Time.deltaTime);
    }

    // 초기값으로 초기화
    public void ResetTiredness() => _data.Reset();

    public void SetTirednessOnDreamEnter() // 잠에 들때 피로도 재설정
    {
        _data.ApplyDreamEnterRecovery(GameDataManager.Instance.Subway.CurrentLineTime);
    }
}
