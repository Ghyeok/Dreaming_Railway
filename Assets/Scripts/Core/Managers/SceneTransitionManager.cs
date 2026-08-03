using UnityEngine;
using UnityEngine.SceneManagement;

public enum FadeType { None, Black, White }

public class SceneTransitionManager : SingletonManagers<SceneTransitionManager>, IManager
{
    private const float FadeOutDuration = 0.3f;
    private const float FadeInDuration = 0.3f;

    private bool _isTransitioning;

    public void Init() { }

    public void GoToMain() => GoToScene(SceneName.Main,FadeType.Black);
    public void GoToStageSelect() => GoToScene(SceneName.StageSelect,FadeType.Black);
    public void ExitFromDream() => GoToScene(SceneName.Subway, FadeType.White);
    public void GoToSubway() => GoToScene(SceneName.Subway, FadeType.Black);

    public void GoToDream() => GoToScene(SceneName.Dream, FadeType.Black);

    private void GoToScene(string sceneName, FadeType fadeType = FadeType.None)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        void LoadScene() => SceneManager.LoadScene(sceneName);
        void Finish() => _isTransitioning = false;

        if (fadeType == FadeType.Black && UI_FadeBlackPanel.Instance != null)
            UI_FadeBlackPanel.Instance.FadeOutThenIn(FadeOutDuration, FadeInDuration, LoadScene, Finish);
        else if (fadeType == FadeType.White && UI_FadeWhitePanel.Instance != null)
            UI_FadeWhitePanel.Instance.FadeOutThenIn(FadeOutDuration, FadeInDuration, LoadScene, Finish);
        else
        {
            LoadScene();
            Finish();
        }
    }
}
