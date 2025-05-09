using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TiredManager : SingletonManagers<TiredManager>
{
    public float maxTired;
    public float currentTired;

    public bool isTiredHalf;

    public override void Awake()
    {
        base.Awake();

        maxTired = 100f;
        currentTired = 30f;
        isTiredHalf = true;
    }

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
                DreamManager.Instance.EnterTheDream();
            }
        }
    }
}
