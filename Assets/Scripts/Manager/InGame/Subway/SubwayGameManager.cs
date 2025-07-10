using UnityEngine;
using UnityEngine.SceneManagement;

/* 플레이어가 지하철에 있을 때의 전체적인 흐름을 관리하는 매니저이다.
 * 게임 모드, 뺨 치기 쿨타임, 뺨 치기 피로도 감소량, 현재 Day 몇인지 등등을 다룬다.
 */
public class SubwayGameManager : SingletonManagers<SubwayGameManager>
{
    public bool isSlapCoolTime;
    public float slapCoolTime;
    public float tiredDecreaseBySlap;
    public int dayCount;
    public int standingCount;
    public bool isCanRetry = false; // 리겜용

    public bool isStopping; // 정차중

    public bool isGameOver;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Awake()
    {
        base.Awake();

        dayCount = 1;
        standingCount = 0;
        slapCoolTime = 5f;

        SoundManager.Instance.bgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME");
        SoundManager.Instance.sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME");
    }

    private void ResetGameManager()
    {
        standingCount = 0;
        slapCoolTime = 5f;
        isGameOver = false;
        tiredDecreaseBySlap = 3f;

        TimerManager.Instance.ResetTimer();

        SoundManager.Instance.SetBGMOn();
        SoundManager.Instance.SetSFXOn();

        SoundManager.Instance.bgmVolume = 1f;
        SoundManager.Instance.sfxVolume = 1f;
    }

    public void ResetGame()
    {
        SubwayGameManager.Instance.ResetGameManager();
        StationManager.Instance.ResetStationManager();
        SubwayPlayerManager.Instance.ResetPlayerManager();
        TiredManager.Instance.ResetTiredManager();
        TransferManager.Instance.ResetTransferManager();
    }

    public int SetDreamMapLengthByAwakenTime()
    // 깨어있던 시간이 50초 이하면 1을 반환, 51초 이상-100초 이하이면 2를 반환
    // 101초-125초 사이면 3을 반환, 126초 이상이면 4를 반환
    {
        if (TimerManager.Instance.awakeTime <= 50f && TimerManager.Instance.awakeTime >= 0f)
            return 1;
        else if (TimerManager.Instance.awakeTime <= 100f && TimerManager.Instance.awakeTime > 50f)
            return 2;
        else if (TimerManager.Instance.awakeTime <= 125f && TimerManager.Instance.awakeTime > 100f)
            return 3;
        else if (TimerManager.Instance.awakeTime > 125f)
            return 4;

        else
        {
            Debug.Log("깨어있던 시간이 음수입니다.");
            return 0;
        }
    }

    private void InitScene()
    {
        TimerManager.Instance.StartTimer();

        GameManager.Instance.gameState = GameManager.GameState.Subway;

        UI_SubwayScene _subway = UIManager.Instance.ShowSceneUI<UI_SubwayScene>("UI_SubwayScene");
        SoundManager.Instance.SubwayBGM();

        DreamManager.Instance.SetDreamTimeSpeedNormal();

        isStopping = false;
        isSlapCoolTime = false;
        tiredDecreaseBySlap = 3f;

        DreamManager.Instance.isInDream = false;

        TimerManager.Instance.awakeTime = 0f;

        if(standingCount == 2)
        {
            standingCount = 0;
        }

        if (isGameOver)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_GameOverPopup");
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
            Debug.Log($"지하철 씬 로드 : {gameObject.name}");
            InitScene();
        }
    }
}