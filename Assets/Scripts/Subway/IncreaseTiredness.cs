using System.Collections;
using UnityEngine;

public class IncreaseTiredness : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SubwayManager.Instance.playerState == SubwayManager.PlayerState.SLEEP &&
            SubwayManager.Instance.currentTired <= SubwayManager.Instance.maxTired)
        {
            StartCoroutine(IncreaseTiredPerOneSecond());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator IncreaseTiredPerOneSecond()
    {
        while (true)
        {
            SubwayManager.Instance.currentTired += SubwayManager.Instance.tiredIncreaseSpeed;
            yield return new WaitForSeconds(1f);

            if (SubwayManager.Instance.currentTired >= SubwayManager.Instance.maxTired &&
                SubwayManager.Instance.playerState == SubwayManager.PlayerState.SLEEP)
            {
                SubwayManager.Instance.playerState = SubwayManager.PlayerState.DEEPSLEEP;
                SubwayManager.Instance.currentTired /= 2;
                // 꿈 속 Scene으로 이동하는 코드
                break;
            }
        }
    }
}
