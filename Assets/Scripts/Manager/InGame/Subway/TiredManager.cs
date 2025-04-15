using System.Collections;
using UnityEngine;

public class TiredManager : SingletonManagers<TiredManager>
{
    public float maxTired;
    public float currentTired;
    public float tiredIncreaseSpeed;

    public bool isTiredHalf;

    public override void Awake()
    {
        base.Awake();

        maxTired = 100f;
        currentTired = 51f;
        tiredIncreaseSpeed = 1f;
        isTiredHalf = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IncreaseTired();
    }

    // Update is called once per frame
    void Update()
    {
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

    private void IncreaseTired()
    {
        if (SubwayPlayerManager.Instance.playerState == SubwayPlayerManager.PlayerState.SLEEP &&
            currentTired <= maxTired)
        {
            StartCoroutine(IncreaseTiredPerOneSecond());
        }
    }


    IEnumerator IncreaseTiredPerOneSecond()
    {
        while (true)
        {
            currentTired += tiredIncreaseSpeed;
            yield return new WaitForSeconds(1f);

            if (currentTired >= maxTired &&
                SubwayPlayerManager.Instance.playerState == SubwayPlayerManager.PlayerState.SLEEP)
            {
                SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.DEEPSLEEP;
                currentTired /= 2;
                // 꿈 속 Scene으로 이동하는 코드
                break;
            }

            if (SubwayPlayerManager.Instance.playerBehave == SubwayPlayerManager.PlayerBehave.FALLASLEEP)
            {
                break;
            }
        }
    }
}
