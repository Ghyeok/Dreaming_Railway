using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamManager : SingletonManagers<DreamManager>, IManager
{
    public bool isInDream;
    public int SlapNum { get; private set; }

    public bool isGameOverInDream;

    private GameData _game;

    public void Init()
    {
        _game = GameDataManager.Instance.Game;

        isGameOverInDream = false;
    }

    public void SetDreamData(int slapNum)
    {
        SlapNum = slapNum;
    }

    public void ResetDreamManager()
    {
        isInDream = false;
        SlapNum = 0;
    }

    public void GameOverInDream()
    {
        isGameOverInDream = true;
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_GameOverPopup");
    }

    private void InitScene()
    {
        isInDream = true;


        if(_game.GameMode == GameMode.Tutorial)
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
