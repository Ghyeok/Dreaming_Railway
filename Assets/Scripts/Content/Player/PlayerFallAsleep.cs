using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;


public class PlayerFallAsleep : MonoBehaviour
{
    public static event Action OnFallAsleepButtonClicked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnterToDream()
    {
        SubwayPlayerManager.Instance.playerBehave = SubwayPlayerManager.PlayerBehave.FALLASLEEP;

        if (SubwayPlayerManager.Instance.playerState == SubwayPlayerManager.PlayerState.SLEEP)
        {
            StartCoroutine(FadeAndLoadScene("InDream_PlayerMove"));
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

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        UI_FadeBlackPanel fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeBlackPanel>();

        fadePanel.Init();
        yield return fadePanel.Fade(0f, 1f, 0.3f); //페이드아웃
        
        // 비동기 씬 로드
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false; // 페이드아웃 끝난 뒤 활성화

        //씬이 거의 다 로드될 때까지 대기
        while (async.progress < 0.9f)
        {
            yield return null;
        }
        async.allowSceneActivation = true;


        yield return null;
    }
}
