using UnityEngine;
using UnityEngine.UI;

public class EnterDream : MonoBehaviour
{
    public Button enterDreamButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enterDreamButton = GetComponent<Button>();
        enterDreamButton.onClick.AddListener(EnterToDream);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EnterToDream()
    {
        if (SubwayManager.Instance.playerState != SubwayManager.PlayerState.STANDING &&
            SubwayManager.Instance.playerState != SubwayManager.PlayerState.DEEPSLEEP)
        {
            SubwayManager.Instance.playerState = SubwayManager.PlayerState.DEEPSLEEP;
        }
    }
}
