using UnityEngine;
using UnityEngine.SceneManagement;

/* 플레이어가 지하철에 있을 때의 전체적인 흐름을 관리하는 매니저이다.
 * 게임 모드, 뺨 치기 쿨타임, 뺨 치기 피로도 감소량, 현재 Day 몇인지 등등을 다룬다.
 */
public class SubwayGameManager : SingletonManagers<SubwayGameManager>, IManager
{
    public bool isSlapCoolTime;
    public float slapCoolTime;
    public float tiredDecreaseBySlap;
    public int standingCount;
    public bool isCanRetry = false; // 리겜용

    public bool isStandingCoolDown; // 입석 쿨타임?
    public bool isStopping; // 정차중

    public bool isGameOver;

    public void Init()
    {
        standingCount = 0;
        isStandingCoolDown = false;
        slapCoolTime = 5f;

        SoundManager.Instance.bgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME");
        SoundManager.Instance.sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME");
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

        if (GameManager.Instance.gameMode == GameManager.GameMode.Infinite)
            tiredDecreaseBySlap = 4f;
        else
            tiredDecreaseBySlap = 3f;

        DreamManager.Instance.isInDream = false;

        TimerManager.Instance.awakeTime = 0f;

        if (standingCount == 2)
        {
            standingCount = 0;
        }

        if (isGameOver)
        {
            GameOver();
        }
    }

    public void ResetGameManager()
    {
        standingCount = 0;
        isStandingCoolDown = false;
        slapCoolTime = 5f;
        isGameOver = false;
        tiredDecreaseBySlap = 3f;

        SoundManager.Instance.SetBGMOn();
        SoundManager.Instance.SetSFXOn();

        SoundManager.Instance.bgmVolume = 1f;
        SoundManager.Instance.sfxVolume = 1f;
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

    private void GameOver()
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_GameOverPopup");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        StageSelectManager.StageSelected -= GameManager.Instance.OnSelectInfiniteMode;
        StageSelectManager.StageSelected += GameManager.Instance.OnSelectInfiniteMode;

    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StageSelectManager.StageSelected -= GameManager.Instance.OnSelectInfiniteMode;
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