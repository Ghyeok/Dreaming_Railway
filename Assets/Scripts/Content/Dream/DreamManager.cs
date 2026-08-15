using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 꿈 씬의 진입 조정자. 상태는 갖지 않는다 — 값은 DreamData가 소유한다.
/// sceneLoaded를 구독해 꿈 씬 진입을 감지하므로, 꿈 씬이 로드되기 전에 반드시 생성돼 있어야 한다.
/// (그래서 ManagerInitializer 목록에 포함된다)
/// </summary>
public class DreamManager : SingletonManagers<DreamManager>, IManager
{
    private DreamData _data; // 실제 데이터

    public DreamData Data => _data;

    public void Init()
    {
        _data = GameDataManager.Instance.Dream;
    }

    public void GameOverInDream()
    {
        _data.SetGameOverInDream();
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_GameOverPopup");
    }

    private void InitScene()
    {
        // Init()보다 먼저 씬이 로드되는 경우를 대비한 방어 (정상 흐름에서는 이미 캐싱돼 있다)
        _data ??= GameDataManager.Instance.Dream;
        _data.EnterDream();

        if (GameDataManager.Instance.Game.GameMode == GameMode.Tutorial)
        {
            TutorialManager.Instance.isSubwayTutorial = false;
            TutorialManager.Instance.isDreamTutorial = true;
            TutorialManager.Instance.startIncreaseTired = false;
            TutorialManager.Instance.dialogState = TutorialManager.DialogState.Dream;

            if (TutorialManager.Instance.dreamIdx < TutorialManager.Instance.exitIdx)
                TutorialManager.Instance.tutorialPopup = UIManager.Instance.ShowPopupUI<UI_TutorialPopup>("UI_TutorialPopup");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SceneName.Dream)
        {
            InitScene();
        }
    }
}
