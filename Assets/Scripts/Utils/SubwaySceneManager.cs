using UnityEngine;

public class SubwaySceneManager : MonoBehaviour
{
    private SubwaySceneRefs _subwaySceneRefs;

    [SerializeField] private TirednessManager tirednessManager;
    [SerializeField] private StationManager stationManager;
    [SerializeField] private TransferManager transferManager;
    [SerializeField] private SubwayPlayerContext subwayPlayerContext;
    [SerializeField] private SubwayRuleManager subwayRuleManager;

    private void Start()
    {
        _subwaySceneRefs = new SubwaySceneRefs()
        {
            tirednessManager = tirednessManager,
            stationManager = stationManager,
            transferManager = transferManager,
            subwayPlayerContext = subwayPlayerContext,
            subwayManager = subwayRuleManager
        };

        tirednessManager.Init(_subwaySceneRefs);
        stationManager.Init(_subwaySceneRefs);
        transferManager.Init(_subwaySceneRefs);
        subwayPlayerContext.Init(_subwaySceneRefs);
        subwayRuleManager.Init(_subwaySceneRefs);

        // 이벤트 구독(씬 이동)
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
