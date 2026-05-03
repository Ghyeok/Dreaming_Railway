using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum FadeType { None, Black }

public class SceneTransitionManager : SingletonManagers<SceneTransitionManager>, IManager
{
    private bool _isTransitioning;

    public void Init() { }

    public void GoToMain() => GoToScene(SceneName.Main);
    public void GoToStageSelect() => GoToScene(SceneName.StageSelect);
    public void GoToSubway() => GoToScene(SceneName.Subway);
    public void GoToDream() => GoToScene(SceneName.Dream);

    public void GoToScene(string sceneName, FadeType fadeType = FadeType.None)
    {
        if (_isTransitioning) return;
        StartCoroutine(TransitionRoutine(sceneName, fadeType));
    }

    private IEnumerator TransitionRoutine(string sceneName, FadeType fadeType)
    {
        _isTransitioning = true;

        if (fadeType == FadeType.Black) // 씬 전환 시 페이드 아웃, 씬 전환 후 새로운 씬에는 따로 처리 안돼있음
        {
            UI_FadeBlackPanel fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeBlackPanel>();
            fadePanel.Init();
            yield return fadePanel.Fade(0f, 1f, 0.3f);
        }

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f)
            yield return null;

        async.allowSceneActivation = true;
        yield return null;

        _isTransitioning = false;
    }
}
