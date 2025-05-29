using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TiredManager : SingletonManagers<TiredManager>
{
    public float maxTired;
    public float currentTired;

    public bool isTiredHalf;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseTired();
        IsTiredHalf();
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


    public void SetTiredAfterDream()
    {
        if (SubwayGameManager.Instance.timer.subwayTime <= 100f)
        {
            currentTired /= 2f;
            SubwayGameManager.Instance.timer.ResetTimer(SubwayGameManager.Instance.timer.subwayTime);
        }
        else if (SubwayGameManager.Instance.timer.subwayTime > 100f)
        {
            currentTired = (currentTired / 2f) * 3f;
            SubwayGameManager.Instance.timer.ResetTimer(SubwayGameManager.Instance.timer.subwayTime);
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

    private void Init()
    {
        maxTired = 100f;
        currentTired = 30f;
        isTiredHalf = true;
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
            Init();
        }
    }
}
