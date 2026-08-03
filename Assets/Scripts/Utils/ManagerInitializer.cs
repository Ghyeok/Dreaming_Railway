using System.Collections.Generic;
using UnityEngine;

public static class ManagerInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // 싱글톤 매니저들의 생성 순서를 정할 수 있음
        IManager[] managersInOrder = new IManager[]
        {
            GameManager.Instance,
            SceneTransitionManager.Instance,
            SoundManager.Instance,
            UIManager.Instance,
            TimerManager.Instance,
            SubwayFlowSystem.Instance
        };

        InitializeManagers(managersInOrder);
    }

    // 내부 로직을 분리하면 유닛테스트로 주입해서 검증하기 쉬워집니다.
    private static void InitializeManagers(IEnumerable<IManager> managers)
    {
        foreach (var manager in managers)
        {
            if (manager == null)
            {
                Debug.LogWarning("ManagerInitializer: found null manager in list, skipping.");
                continue;
            }

            try
            {
                manager.Init();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ManagerInitializer: Init failed for {manager.GetType().Name}. Exception: {ex}");
                // 계속 진행: 한 매니저 실패가 전체 초기화를 중단하지 않도록 합니다.
            }
        }
    }
}
