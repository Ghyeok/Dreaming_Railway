using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


// 지하철 내 플레이어의 행동, 상태를 관리하는 매니저이다.
public class SubwayPlayerManager : SingletonManagers<SubwayPlayerManager>
{
    public enum PlayerState
    {
        NONE,
        STANDING, // 입석
        SLEEP, // 졸고 있음, 지속적으로 피로도 증가
        DEEPSLEEP, // 피로도 100, 꿈 속 진입
        GAMEOVER,
    }

    public enum PlayerBehave
    {
        NONE,
        SLAP, // 뺨 때리기
        FALLASLEEP, // 즉시 잠들기
        TRANSFER, // 환승하기
        GETOFF, // 목적지에 내리기
    }

    public GameObject subwayPlayer;

    public PlayerState playerState;
    public PlayerBehave playerBehave;
    public int slapNum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitScene()
    {
        slapNum = 0;
        playerState = PlayerState.SLEEP;
        playerBehave = PlayerBehave.NONE;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestSubwayScene")
        {
            Debug.Log($"지하철 씬 로드: {gameObject.name}");
            InitScene();
        }
    }
}
