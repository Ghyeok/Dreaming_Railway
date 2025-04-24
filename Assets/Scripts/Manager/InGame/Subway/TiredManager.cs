using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TiredManager : SingletonManagers<TiredManager>
{
    public float maxTired;
    public float currentTired;
    private float tiredTimer;

    public bool isTiredHalf;

    public override void Awake()
    {
        base.Awake();

        maxTired = 100f;
        currentTired = 30f;
        tiredTimer = 0f;
        isTiredHalf = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseTiredOneSecond();
        CheckTired();
    }

    private void CheckTired()
    {
        currentTired = Mathf.Clamp(currentTired, 0, maxTired);

        if (currentTired > maxTired / 2)
        {
            isTiredHalf = true;
        }
        else
        {
            isTiredHalf = false;
        }
    }

    private void IncreaseTiredOneSecond()
    {
        if (SubwayPlayerManager.Instance.playerState == SubwayPlayerManager.PlayerState.SLEEP)
        {
            tiredTimer += Time.deltaTime;
            if (tiredTimer >= 1f)
            {
                currentTired += 1f;
                tiredTimer = 0f;   
                
                if(currentTired >= maxTired)
                {
                    SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.DEEPSLEEP;
                    currentTired /= 2; // 감소하는 피로도 수정 필요
                    SceneManager.LoadScene("InDream_PlayerMove");
                }
            }
        }
    }
}
