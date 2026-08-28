using UnityEngine;

/// <summary>
/// 플레이 타임을 틱으로 굴리는 시스템. 상태는 갖지 않는다.
/// 지하철 <-> 꿈 씬 전환 사이에도 시간이 계속 흘러야 하므로 DontDestroyOnLoad 싱글톤이다.
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

        _data.Tick(Time.deltaTime);
    }
}
