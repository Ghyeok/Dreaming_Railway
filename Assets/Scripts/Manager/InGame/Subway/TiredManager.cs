using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TiredManager : SingletonManagers<TiredManager>, IManager
{ 
    public float maxTired;
    public float currentTired;

    public bool isTiredHalf; // true면 조는 모션, false면 멀쩡한 모션

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
        if(currentTired < maxTired / 2)
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
        if (SubwayPlayerManager.Instance.playerState == SubwayPlayerManager.PlayerState.SLEEP)
        {
            currentTired += Time.deltaTime;
            if (currentTired >= maxTired)
            {
                SceneManager.LoadScene("InDream_PlayerMove");
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
        }
    }
}
