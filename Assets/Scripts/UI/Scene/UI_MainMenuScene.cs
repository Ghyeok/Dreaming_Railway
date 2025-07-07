using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenuScene : UI_Scene
{
    public enum Buttons
    {
        NormalModeButton,

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        GameObject normal = GetButton((int)Buttons.NormalModeButton).gameObject;
        AddUIEvent(normal, NormalMode);

        SoundManager.Instance.PlayAudioClip("TitleTheme", Define.Sounds.BGM);
    }

    private void InitScene()
    {
        GameManager.Instance.gameState = GameManager.GameState.Main;
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
        if (scene.name == "MainScene")
        {
            Debug.Log($"메인 씬 로드 : {gameObject.name}");
            InitScene();
        }
    }

    private void NormalMode(PointerEventData data)
    {
        SceneManager.LoadScene("TestSubwayScene");
    }
}
