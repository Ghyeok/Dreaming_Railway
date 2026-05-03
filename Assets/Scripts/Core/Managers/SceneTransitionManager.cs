using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum FadeType { None, Black, White }

public class SceneTransitionManager : SingletonManagers<SceneTransitionManager>, IManager
{
    private bool _isTransitioning;

    public void Init() { }

    public void GoToMain() => GoToScene(SceneName.Main);
    public void GoToStageSelect() => GoToScene(SceneName.StageSelect);
    public void GoToSubway() => GoToScene(SceneName.Subway, FadeType.White);
    public void GoToDream() => GoToScene(SceneName.Dream, FadeType.Black);

    public void GoToScene(string sceneName, FadeType fadeType = FadeType.None)
    {
        if (_isTransitioning) return;
        StartCoroutine(TransitionRoutine(sceneName, fadeType));
    }

    private IEnumerator TransitionRoutine(string sceneName, FadeType fadeType)
    {
        _isTransitioning = true;

        if (fadeType == FadeType.Black)
        {
            UI_FadeBlackPanel fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeBlackPanel>();
            fadePanel.Init();
            yield return fadePanel.Fade(0f, 1f, 0.3f);
        }
        else if (fadeType == FadeType.White)
        {
            UI_FadeWhitePanel fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeWhitePanel>();
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
