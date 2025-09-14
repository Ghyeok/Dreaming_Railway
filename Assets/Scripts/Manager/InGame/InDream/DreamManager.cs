using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamManager : SingletonManagers<DreamManager>, IManager
{
    public bool isInDream;

    public float dreamTimeSpeed;
    private float mindreamTimeSpeed;
    private float maxdreamTimeSpeed;

    public void Init()
    {

    }

    public void ResetDreamManager()
    {
        isInDream = false;
    }

    public void RandomDreamTimeSpeed()
    {
        mindreamTimeSpeed = 3.1f;
        maxdreamTimeSpeed = 4.1f;

        dreamTimeSpeed = UnityEngine.Random.Range(mindreamTimeSpeed, maxdreamTimeSpeed);

    }

    public void SetDreamTimeSpeedNormal()
    {
        dreamTimeSpeed = 1f;
    }

    public void GameOverInDream()
    {
        GameManager.Instance.isGameOverInDream = true;
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_GameOverPopup");
    }

    private void InitScene()
    {
        GameManager.Instance.gameState = GameManager.GameState.Dream;

        isInDream = true;
        RandomDreamTimeSpeed();
        SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.DEEPSLEEP;
        TiredManager.Instance.SetTiredAfterDream();
        SoundManager.Instance.PlayAudioClip("DreamMusic", Define.Sounds.BGM);

        if(GameManager.Instance.gameMode == GameManager.GameMode.Tutorial)
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
        if (scene.name == "InDream_PlayerMove")
        {
            UIManager.Instance.ShowSceneUI<UI_Scene>("UI_NonGameOverScene");
            InitScene();
        }
    }
}
