using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerFallAsleep : MonoBehaviour
{
    private SubwayGameManager _game; // 의존성 주입 고려중
    public static event Action OnFallAsleepButtonClicked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
            SceneManager.LoadScene("InDream_PlayerMove");
        }
    }

    public static void TriggerFallAsleep()
    {
        OnFallAsleepButtonClicked?.Invoke();
    }

    private void OnEnable()
    {
        OnFallAsleepButtonClicked += EnterToDream;
    }

    private void OnDisable()
    {
        OnFallAsleepButtonClicked -= EnterToDream;
    }
}
