using UnityEngine;

[DefaultExecutionOrder(-100)]
public class DreamSceneManager : MonoBehaviour
{
    [SerializeField] private FogSpawn fogSpawn;
    [SerializeField] private MapXSpawn mapXSpawn;
    [SerializeField] private MapYSpawn mapYSpawn;
    [SerializeField] private Camera_move cameraMove;

    private void Start()
    {
        // ㅡㅡㅡㅡ 초기화 순서 제어 및 의존성 주입 ㅡㅡㅡㅡ
        cameraMove.Init(mapXSpawn);

        // ㅡㅡㅡㅡ 이벤트 구독 ㅡㅡㅡㅡ
        FogSpawn.OnOriginDecided += HandleDirectionDecided;

        // ㅡㅡㅡㅡ 그 외 일회성 메서드 ㅡㅡㅡㅡ
        SoundManager.Instance.DreamBGM();
        GameDataManager.Instance.Subway.SetFlowSpeed(true);
    }

    private void OnDestroy()
    {
        FogSpawn.OnOriginDecided -= HandleDirectionDecided;
    }

    private void HandleDirectionDecided(FogOrigin origin, FogMovement fogMovement)
    {
        mapXSpawn.SetOrigin(origin);
        mapYSpawn.SetOrigin(origin);
        cameraMove.SetOrigin(origin);
        fogMovement.SetOrigin(origin);
    }
}
