using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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

        StartCoroutine(FadeAndLoadScene("StageSelect"));
    }

    private void InfiniteModeOnClicked(PointerEventData data)
    {
        GameManager.Instance.gameMode = GameManager.GameMode.Infinite;
        StageSelectManager.Instance.InvokeStageSelect();

        StartCoroutine(FadeAndLoadScene("TestSubwayScene"));
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
            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, targetPos, slideSpeed * Time.deltaTime);
        }
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
