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
            ScriptManager.Instance,
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
    }
}
