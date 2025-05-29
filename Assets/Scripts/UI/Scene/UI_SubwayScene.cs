using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SubwayScene : UI_Scene
{
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
        // base.Init();

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

        // Image 관련 함수들, 쓰일수도 있으니 미리 가지고 있자
        GameObject backgroundBlackImage = GetImage((int)Images.BackgroundBlackImage).gameObject;
        GameObject backgroundWhiteImage = GetImage((int)Images.BackgroundWhiteImage).gameObject;
        GameObject backgroundImage = GetImage((int)Images.BackgroundImage).gameObject;
        GameObject backgroundLineImage = GetImage((int)Images.BackgroundLineImage).gameObject;
        GameObject timerImage = GetImage((int)Images.TimerImage).gameObject;

        // Text 관련 함수들, 쓰일수도 있으니 미리 가지고 있자
        TextMeshProUGUI timerText = GetText((int)Texts.TimerText);
        SubwayGameManager.Instance.timer.timerText = timerText;

        GameObject transferText = GetText((int)Texts.TransferText).gameObject;
        GameObject slapText = GetText((int)Texts.SlapText).gameObject;
    }

    private void PauseButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_PausePopup");
        Debug.Log("일시정지!");
        Time.timeScale = 0;
    }

    private void SetTransferText()
    {
        GetText((int)Texts.TransferText).text = $"환승까지 <size=300%>{StationManager.Instance.transferStationIdx - StationManager.Instance.currentStationIdx}</size>역";
    }

    private void SetSlapText()
    {
        GetText((int)Texts.SlapText).text = $"{SubwayPlayerManager.Instance.slapNum}";
    }

    private void SubwayCharacterSleepingMotion()
    {
        Sprite sprite = GetImage((int)Images.PlayerImage).sprite;
    }
}
