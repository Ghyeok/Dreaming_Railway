using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SubwayScene : UI_Scene
{
    public GameObject subwayPlayer;

    [SerializeField]
    Animator anim;

    public enum Buttons
    {
        StandingButton,
        FallAsleepButton,
        SlapButton,
        PauseButton,
        // 필요한 버튼 추가..
    }

    public enum Images
    {
        PlayerImage,
        BackgroundSubwayImage,
        BackgroundBlackImage,
        BackgroundWhiteImage,
        BackgroundImage,
        BackgroundLineImage,
        TimerImage,
        // 필요한 이미지 추가..
    }

    public enum Texts
    {
        TransferText,
        NextTransferText,
        TimerText,
        SlapText,
        // 필요한 텍스트 추가..
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        SetTransferText();
        SetSlapText();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        subwayPlayer = GetImage((int)Images.PlayerImage).gameObject;
        SubwayPlayerManager.Instance.subwayPlayer = subwayPlayer;
        anim = subwayPlayer.GetComponent<Animator>();

        GameObject pause = GetButton((int)Buttons.PauseButton).gameObject;
        AddUIEvent(pause, PauseButtonOnClicked, Define.UIEvent.Click);

        GameObject stand = GetButton((int)Buttons.StandingButton).gameObject;
        AddUIEvent(stand, SetStandingButtonToSkip, Define.UIEvent.Click);

        GameObject slap = GetButton((int)Buttons.SlapButton).gameObject;
        AddUIEvent(slap, data => PlayerSlap.TriggerSlap(), Define.UIEvent.Click);

        GameObject fallAsleep = GetButton((int)Buttons.FallAsleepButton).gameObject;
        AddUIEvent(fallAsleep, data => PlayerFallAsleep.TriggerFallAsleep(), Define.UIEvent.Click);

        TextMeshProUGUI timerText = GetText((int)Texts.TimerText);
        SubwayGameManager.Instance.timer.timerText = timerText;
    }

    private void PauseButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_PausePopup");
        Debug.Log("일시정지!");
        Time.timeScale = 0;
    }

    private void SetTransferText()
    {
        int line = StationManager.Instance.currentLineIdx;
        GetText((int)Texts.TransferText).text = $"환승까지 <size=300%>{StationManager.Instance.subwayLines[line].transferIdx - StationManager.Instance.currentStationIdx + 1}</size>역";

        if ((line + 1) != StationManager.Instance.subwayLines.Count)
            GetText((int)Texts.NextTransferText).text = $"환승까지 <size=300%>{StationManager.Instance.subwayLines[line + 1].transferIdx + 1}</size>역";
        else
            GetText((int)Texts.NextTransferText).text = null;
    }

    private void SetSlapText()
    {
        GetText((int)Texts.SlapText).text = $"{SubwayPlayerManager.Instance.slapNum}";
    }

    private void SetStandingButtonToSkip(PointerEventData data)
    {
        if (SubwayGameManager.Instance.standingCount == 0)
        {
            // 초기 설정
            SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.STANDING;
            SoundManager.Instance.PlayAudioClip("Standing", Define.Sounds.SFX);
            TiredManager.Instance.currentTired = 99;
            anim.SetTrigger("isStanding");

            // 1. 다른 버튼 비활성화
            CanvasGroup cg = GetButton((int)Buttons.FallAsleepButton).gameObject.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            cg = GetButton((int)Buttons.SlapButton).gameObject.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            // 2. 스킵 버튼으로 변경, 이벤트 연결
            GetButton((int)Buttons.StandingButton).GetComponent<Image>().sprite = Resources.Load<Sprite>("Arts/UIs/Subway/Player/Button_Skip");
            GameObject stand = GetButton((int)Buttons.StandingButton).gameObject;
            ClearUIEvent(stand);
            AddUIEvent(stand, data => PlayerStanding.TriggerStanding(), Define.UIEvent.Click);
        }
    }
}
