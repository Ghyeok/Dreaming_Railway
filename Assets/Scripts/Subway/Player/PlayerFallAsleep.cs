using UnityEngine;
using UnityEngine.UI;

public class PlayerFallAsleep : MonoBehaviour
{
    public Button fallAsleepButton;
    private SubwayGameManager _game; // 의존성 주입 고려중

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
        SubwayPlayerManager.Instance.playerBehave = SubwayPlayerManager.PlayerBehave.FALLASLEEP;

        if (SubwayPlayerManager.Instance.playerState != SubwayPlayerManager.PlayerState.STANDING &&
            SubwayPlayerManager.Instance.playerState != SubwayPlayerManager.PlayerState.DEEPSLEEP)
        {
            SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.DEEPSLEEP;
            // 꿈 속 Scene으로 이동하는 코드
        }
    }
}
