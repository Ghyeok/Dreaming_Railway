using UnityEngine;

public class SubwaySceneBinder : MonoBehaviour
{
    private SubwaySceneRefs _subwaySceneRefs;

    [SerializeField] private TirednessManager tirednessManager;
    [SerializeField] private StationManager stationManager;
    [SerializeField] private TransferManager transferManager;

    private void Start()
    {
        _subwaySceneRefs = new SubwaySceneRefs()
        {
            tirednessManager = tirednessManager,
            stationManager = stationManager,
            transferManager = transferManager
        };

        tirednessManager.Init(_subwaySceneRefs);
        stationManager.Init(_subwaySceneRefs);
        transferManager.Init(_subwaySceneRefs);

        // 이벤트 구독
        tirednessManager.OnTiredMaxed += MoveToDreamScene;
    }

    public void MoveToDreamScene()
    {

    }

    private void OnDestroy()
    {
        tirednessManager.OnTiredMaxed -= MoveToDreamScene;
    }

}
