using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MainScene : UI_Scene
{
    [Tooltip("Tap To Start")]
    private float blinkSpeed = 0.8f;
    private float blinkTimer;
    private Vector2 targetPos;
    private Vector2 hiddenPos;
    private RectTransform rect;
    private float slideSpeed = 3000f;
    private bool isTapped;

    [SerializeField] private Sprite stageLock;
    [SerializeField] private Sprite stageUnlock;

    private Image _tapToStartImage;
    private Image _infiniteButtonImage;
    private Button _infiniteButton;
    private CanvasGroup _infiniteButtonCG;

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

    void Start()
    {
        Init();
    }

    void Update()
    {
        BlinkTapToStart();
        SlideMainMenu();
    }

    public override void Init()
    {
        base.Init();

        GameDataManager.Instance.Game.SetGameMode(GameMode.None);
        SoundManager.Instance.MainBGM();

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        _tapToStartImage = GetImage((int)Images.TapToStart);
        _tapToStartImage.gameObject.SetActive(true);

        _infiniteButton = GetButton((int)Buttons.InfiniteModeButton);
        _infiniteButtonImage = _infiniteButton.GetComponent<Image>();
        _infiniteButtonCG = Util.GetOrAddComponent<CanvasGroup>(_infiniteButton.gameObject);

        AddUIEvent(GetButton((int)Buttons.NormalModeButton).gameObject, NormalModeOnClicked);
        AddUIEvent(_infiniteButton.gameObject, InfiniteModeOnClicked);
        AddUIEvent(GetButton((int)Buttons.SettingButton).gameObject, SettingButtonOnClicked);
        AddUIEvent(GetButton((int)Buttons.CreditButton).gameObject, CreditButtonOnClicked);
        AddUIEvent(GetButton((int)Buttons.ExitButton).gameObject, ExitButtonOnClicked);

        targetPos = _tapToStartImage.rectTransform.anchoredPosition;
        hiddenPos = new Vector2(targetPos.x, targetPos.y - 1080f);
        rect = GetImage((int)Images.MainMenu).rectTransform;
        rect.anchoredPosition = hiddenPos;

        LoadInfiniteModeLock();
    }

    private void NormalModeOnClicked(PointerEventData data)
    {
        GameDataManager.Instance.Game.SetGameMode(GameMode.NormalMode);
        SceneTransitionManager.Instance.GoToStageSelect();
    }

    private void InfiniteModeOnClicked(PointerEventData data)
    {
        GameDataManager.Instance.Game.SetGameMode(GameMode.InfiniteMode);

        // 무한 모드는 StartDay()를 거치지 않으므로 여기서 직접 초기화한다
        GameDataManager.Instance.ResetForNewRun();

        SceneTransitionManager.Instance.GoToSubway();
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
        blinkTimer += blinkSpeed * Time.deltaTime;
        Color color = _tapToStartImage.color;
        color.a = Mathf.Abs(Mathf.Sin(blinkTimer));
        _tapToStartImage.color = color;
    }

    private void SlideMainMenu()
    {
        if (!isTapped && (Input.GetMouseButton(0) || Input.touchCount > 0))
        {
            isTapped = true;
            _tapToStartImage.gameObject.SetActive(false);
        }

        if (isTapped)
        {

            rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, targetPos, slideSpeed * Time.deltaTime);
        }
    }

    private void LoadInfiniteModeLock()
    {
        int maxClearDay = GameDataManager.Instance.Game.MaxClearDay;
        SetInfiniteButtonState(maxClearDay >= 5);
    }

    private void SetInfiniteButtonState(bool unlocked)
    {
        _infiniteButtonImage.sprite = unlocked ? stageUnlock : stageLock;
        _infiniteButtonImage.raycastTarget = unlocked;
        _infiniteButton.interactable = unlocked;
        _infiniteButtonCG.blocksRaycasts = unlocked;
        _infiniteButtonCG.interactable = unlocked;
    }
}
