using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TiredManager : SingletonManagers<TiredManager>
{
    public float maxTired;
    public float currentTired;

    public bool isTiredHalf; // true면 조는 모션, false면 멀쩡한 모션


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
        if (SubwayGameManager.Instance.timer.awakeTime <= 100f)
        {
            currentTired /= 2f;
        }
        else if (SubwayGameManager.Instance.timer.awakeTime > 100f)
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

    public override void Awake()
    {
        base.Awake();
        currentTired = 30f;
        maxTired = 100f;
    }

    private void InitScene()
    {

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
