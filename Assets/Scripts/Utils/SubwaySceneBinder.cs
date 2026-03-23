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
    }
}
