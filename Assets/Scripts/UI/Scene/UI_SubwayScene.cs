using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SubwayScene : UI_Scene
{
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
        SubwayCharacterSleepingMotion();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GameObject pause = GetButton((int)Buttons.PauseButton).gameObject;
        AddUIEvent(pause, PauseButtonOnClicked, Define.UIEvent.Click);

        GameObject stand = GetButton((int)Buttons.StandingButton).gameObject;
        AddUIEvent(stand, data => PlayerStanding.TriggerStanding(), Define.UIEvent.Click);

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
        GetText((int)Texts.TransferText).text = $"환승까지 <size=300%>{StationManager.Instance.subwayLines[line].transferIdx - StationManager.Instance.currentStationIdx}</size>역";

        if ((line + 1) != StationManager.Instance.subwayLines.Count)
            GetText((int)Texts.NextTransferText).text = $"환승까지 <size=300%>{StationManager.Instance.subwayLines[line + 1].transferIdx}</size>역";
        else
            GetText((int)Texts.NextTransferText).text = null;
    }

    private void SetSlapText()
    {
        GetText((int)Texts.SlapText).text = $"{SubwayPlayerManager.Instance.slapNum}";
    }

    private void SubwayCharacterSleepingMotion()
    {
        anim = GetImage((int)Images.PlayerImage).gameObject.GetComponent<Animator>();
        anim.SetFloat("tired", TiredManager.Instance.currentTired);
    }
}
