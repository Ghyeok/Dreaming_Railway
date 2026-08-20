using UnityEngine;

/// <summary>
/// 플레이 타임을 매 프레임 구동하는 시스템. 상태는 갖지 않는다.
/// 일시정지 제어가 필요한 쪽은 이 클래스를 거치지 말고 TimerData를 직접 호출한다.
/// 씬 전환 중에도 시간이 흘러야 하므로 DontDestroyOnLoad 싱글톤이다.
/// </summary>
public class TimerSystem : SingletonManagers<TimerSystem>, IManager
{
    private TimerData _data;

    public void Init()
    {
        _data = GameDataManager.Instance.Timer;
    }

    private void Update()
    {
        if (_data == null) return;

        _data.Tick(Time.deltaTime); // IsPaused 판단은 데이터 안에서 한다
    }
}
