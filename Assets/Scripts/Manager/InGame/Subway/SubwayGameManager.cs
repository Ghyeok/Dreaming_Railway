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
    public bool isStandingCoolDown; // 입석 쿨타임?
    public bool isStopping; // 정차중

    public bool isGameOver;

    public void Init()
    {
        standingCount = 0;
        isStandingCoolDown = false;
        slapCoolTime = 5f;

        SoundManager.Instance.SetBGMVolume(PlayerPrefs.GetFloat("BGM_VOLUME", 1f));
        SoundManager.Instance.SetSFXVolume(PlayerPrefs.GetFloat("SFX_VOLUME", 1f));
    }

    private void InitScene()
    {
        TimerManager.Instance.StartTimer(); // 타이머 시작
        GameManager.Instance.gameState = GameManager.GameState.Subway; // 게임 상태를 지하철로

        UI_SubwayScene _subway = UIManager.Instance.ShowSceneUI<UI_SubwayScene>("UI_SubwayScene"); // 지하철 UI 출력
        SoundManager.Instance.SubwayBGM(); // 지하철 BGM 재생

        DreamManager.Instance.SetDreamTimeSpeedNormal(); // 시간 흐름 속도 1로 초기화

        isStopping = false;
        isSlapCoolTime = false;

        if (GameManager.Instance.gameMode == GameManager.GameMode.Infinite)
        {
            tiredDecreaseBySlap = 4f;
        }
        else
        {
            tiredDecreaseBySlap = 3f;
        }

        DreamManager.Instance.isInDream = false; // 꿈 속이 아니므로

        TimerManager.Instance.awakeTime = 0f; // 지하철 씬이 로드되는 순간 깨어있는 시간 0으로 초기화

        if (isGameOver && GameManager.Instance.gameState == GameManager.GameState.Subway) // 꿈 속에서 지하철로 돌아 왔을때, 환승역을 지나친 상태라면 게임오버 판정
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

    public void GameOver()
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_GameOverPopup");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        var gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            StageSelectManager.StageSelected -= GameManager.Instance.OnSelectInfiniteMode;
            StageSelectManager.StageSelected += GameManager.Instance.OnSelectInfiniteMode;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        var gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            StageSelectManager.StageSelected -= GameManager.Instance.OnSelectInfiniteMode;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestSubwayScene")
        {
            InitScene();
        }
    }
}