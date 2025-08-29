using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TiredManager : SingletonManagers<TiredManager>, IManager
{
    public float maxTired;
    public float currentTired;

    public bool isTiredHalf; // true면 조는 모션, false면 멀쩡한 모션
    private bool isSceneLoading = false;

    public void Init()
    {
        currentTired = 30f;
        maxTired = 100f;
    }

    private void InitScene()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.Subway &&
            GameManager.Instance.gameState != GameManager.GameState.Dream)
            return;

        SubwayCharacterSleepingMotion();
        IncreaseTired();
        IsTiredHalf();
    }

    public void ResetTiredManager()
    {
        currentTired = 30f;
        maxTired = 100f;
    }

    private void SubwayCharacterSleepingMotion()
    {
        if (SubwayPlayerManager.Instance.subwayPlayer != null)
        {
            Animator anim = SubwayPlayerManager.Instance.subwayPlayer.GetComponent<Animator>();
            anim.SetFloat("tired", TiredManager.Instance.currentTired);
        }
    }

    private void IsTiredHalf()
    {
        if (currentTired < maxTired / 2)
        {
            isTiredHalf = false;
        }
        else
        {
            isTiredHalf = true;
        }
    }


    public void SetTiredAfterDream() // 잠에 들때 피로도 재설정
    {
        if (TimerManager.Instance.awakeTime <= 100f)
        {
            currentTired /= 2f;
        }
        else if (TimerManager.Instance.awakeTime > 100f)
        {
            currentTired = (currentTired / 2f) * 3f;
        }
    }

    private void IncreaseTired()
    {
        if (SubwayPlayerManager.Instance.playerState == SubwayPlayerManager.PlayerState.SLEEP &&
            !SubwayGameManager.Instance.isGameOver &&
            (GameManager.Instance.gameMode != GameManager.GameMode.Tutorial && !TutorialManager.Instance.startFlowTime) || // 튜토리얼 아닐때는 변수 체크 X
             GameManager.Instance.gameMode == GameManager.GameMode.Tutorial && TutorialManager.Instance.startIncreaseTired) 
        {
            currentTired += Time.deltaTime;

            if (!isSceneLoading && currentTired >= maxTired)
            {
                isSceneLoading = true;
                StartCoroutine(FadeAndLoadScene("InDream_PlayerMove"));
            }
        }
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
            InitScene();
            isSceneLoading = false;
        }
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        UI_FadeBlackPanel fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeBlackPanel>();

        fadePanel.Init();
        yield return fadePanel.Fade(0f, 1f, 0.3f); //페이드아웃
        
        // 비동기 씬 로드
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false; // 페이드아웃 끝난 뒤 활성화

        //씬이 거의 다 로드될 때까지 대기
        while (async.progress < 0.9f)
        {
            yield return null;
        }
        async.allowSceneActivation = true;


        yield return null;
    }
}
