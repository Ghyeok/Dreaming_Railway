using UnityEngine;
using UnityEngine.UI;

public class FallAsleep : MonoBehaviour
{
    public Button fallAsleepButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fallAsleepButton = GetComponent<Button>();
        fallAsleepButton.onClick.AddListener(EnterToDream);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void EnterToDream()
    {
        SubwayManager.Instance.playerBehave = SubwayManager.PlayerBehave.FALLASLEEP;

        if (SubwayManager.Instance.playerState != SubwayManager.PlayerState.STANDING &&
            SubwayManager.Instance.playerState != SubwayManager.PlayerState.DEEPSLEEP)
        {
            SubwayManager.Instance.playerState = SubwayManager.PlayerState.DEEPSLEEP;
            // 꿈 속 Scene으로 이동하는 코드
        }
    }
}
