using System.Collections.Generic;
using UnityEngine;

public static class ManagerInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        IManager[] managersInOrder = new IManager[]
        {
            GameManager.Instance,
            ResolutionManager.Instance,
            SoundManager.Instance,
            UIManager.Instance,
            StageSelectManager.Instance,
            TutorialManager.Instance,
            ScriptManager.Instance,
            TransferManager.Instance,
            StationManager.Instance,
            TiredManager.Instance,
            SubwayGameManager.Instance,
            SubwayPlayerManager.Instance,
            DreamManager.Instance,
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
