using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainMenuScene : UI_Scene
{
    private float blinkSpeed = 0.8f;
    private float bilnkTimer;

    [SerializeField]
    private Vector2 targetPos;
    [SerializeField]
    private Vector2 hiddenPos;
    private RectTransform rect;
    private float slideSpeed = 3000f;
    private bool isTapped;

    public enum Buttons
    {
        NormalModeButton,
        InfiniteModeButton,
        SettingButton,
        CreditButton,
        ExitButton,
    }

    public enum Images
    {
        TapToStart,
        MainMenu,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        BlinkTapToStart();
        SlideMainMenu();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        GetImage((int)Images.TapToStart).gameObject.SetActive(true);

        GameObject normal = GetButton((int)Buttons.NormalModeButton).gameObject;
        AddUIEvent(normal, NormalModeOnClicked);

        GameObject infinite = GetButton((int)Buttons.InfiniteModeButton).gameObject;
        AddUIEvent(infinite, InfiniteModeOnClicked);

        GameObject setting = GetButton((int)Buttons.SettingButton).gameObject;
        AddUIEvent(setting, SettingButtonOnClicked);

        GameObject credit = GetButton((int)Buttons.CreditButton).gameObject;
        AddUIEvent(credit, CreditButtonOnClicked);

        GameObject exit = GetButton((int)Buttons.ExitButton).gameObject;
        AddUIEvent(exit, ExitButtonOnClicked);

        SoundManager.Instance.PlayAudioClip("TitleTheme", Define.Sounds.BGM);

        targetPos = GetImage((int)Images.TapToStart).rectTransform.anchoredPosition;
        hiddenPos = new Vector2(targetPos.x, targetPos.y - 1080f);
        rect = GetImage((int)Images.MainMenu).rectTransform;
        rect.anchoredPosition = hiddenPos;

    }

    private void InitScene()
    {
        GameManager.Instance.gameState = GameManager.GameState.Main;
        GameManager.Instance.gameMode = GameManager.GameMode.None;
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

    private void NormalModeOnClicked(PointerEventData data)
    {
        GameManager.Instance.gameMode = GameManager.GameMode.Normal;
        SceneManager.LoadScene("TestSubwayScene");
    }

    private void InfiniteModeOnClicked(PointerEventData data)
    {
        GameManager.Instance.gameMode = GameManager.GameMode.Infinite;
        SceneManager.LoadScene("TestSubwayScene");
    }

    private void SettingButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_SettingPopup");
    }

    private void CreditButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_CreditPopup");
    }
    
    private void ExitButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.OnExitButton();
    }

    private void BlinkTapToStart()
    {
        bilnkTimer += blinkSpeed * Time.deltaTime;
        Image tts = GetImage((int)Images.TapToStart);

        Color color = tts.color;
        color.a = Mathf.Abs(Mathf.Sin(bilnkTimer));
        tts.color = color;
    }

    private void SlideMainMenu()
    {
        if (!isTapped && Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            isTapped = true;
            GetImage((int)Images.TapToStart).gameObject.SetActive(false);
        }

        if (isTapped)
        {
            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition,targetPos, slideSpeed * Time.deltaTime);
        }
    }
}
