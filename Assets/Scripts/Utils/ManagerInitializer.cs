using System.Collections.Generic;
using UnityEngine;

public class ManagerInitializer : MonoBehaviour
{
    private void Awake()
    {
        List<IManager> managersInOrder = new()
        {
            GameManager.Instance,
            ResolutionManager.Instance,
            SoundManager.Instance,
            UIManager.Instance,
            StageSelectManager.Instance,
            TutorialManager.Instance,
            TransferManager.Instance,
            StationManager.Instance,
            TiredManager.Instance,
            SubwayGameManager.Instance,
            SubwayPlayerManager.Instance,
            DreamManager.Instance,
        };

        foreach (var manager in managersInOrder)
        {
            manager.Init();
        }

        Debug.Log("모든 매니저 초기화 완료");
    }
}
