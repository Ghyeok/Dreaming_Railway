using System;
using System.Collections;
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
        MaxCount,
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
        SlapCoolTimeImage,
        StandingCoolTimeImage,
        TutorialFallAsleepImage,
        TutorialSlapImage,
        TutorialStandingImage,
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
        UI_FadeBlackPanel fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeBlackPanel>();
        fadePanel.StartFadeIn(0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        SetTransferText();
        SetSlapText();
        ShowStandingCoolDown();

        if (GameManager.Instance.gameMode == GameManager.GameMode.Tutorial)
            TutorialButtonBlocker();
    }

    private void OnEnable()
    {
        PlayerSlap.OnSlapSuccess -= OnSlapButtonClicked;
        PlayerSlap.OnSlapSuccess += OnSlapButtonClicked;

        // 현재 눌린 UI 상태 초기화
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnDisable()
    {
        PlayerSlap.OnSlapSuccess -= OnSlapButtonClicked;
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
        TimerManager.Instance.timerText = timerText;
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
        if (!SubwayGameManager.Instance.isStandingCoolDown &&
            StationManager.Instance.currentStationIdx != StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].transferIdx)
        {
            // 초기 설정
            SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.STANDING;
            SoundManager.Instance.PlayAudioClip("Standing", Define.Sounds.SFX);
            TiredManager.Instance.currentTired = 99.9f;
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

            StartCoroutine(StandingTutorial());
        }
    }


    IEnumerator StandingTutorial()
    {
        if (GameManager.Instance.gameMode == GameManager.GameMode.Tutorial)
        {
            CanvasGroup cg = GetButton((int)Buttons.StandingButton).gameObject.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            yield return new WaitForSecondsRealtime(2f);

            cg.blocksRaycasts = true;

            TutorialManager.Instance.isStandingTutorial = false;
            TutorialManager.Instance.tutorialPopup.gameObject.SetActive(true);
            TutorialManager.Instance.tutorialPopup.AdvanceDialog();
        }
    }

    IEnumerator ShowSlapCoolTime()
    {
        float startTime = Time.time;
        float coolTime = SubwayGameManager.Instance.slapCoolTime;
        Image slap = GetImage((int)Images.SlapCoolTimeImage);

        slap.fillAmount = 1f; // 뺨 때리기 누르면 1로 초기화

        while (Time.time < startTime + coolTime)
        {
            float elapsed = Time.time - startTime;
            float ratio =  1f - (elapsed / coolTime);
            slap.fillAmount = ratio;
            yield return null;
        }

        slap.fillAmount = 0f;
    }

    private void ShowStandingCoolDown()
    {
        Image stand = GetImage((int)Images.StandingCoolTimeImage);

        if (SubwayGameManager.Instance.isStandingCoolDown)
        {
            stand.fillAmount = 1 - SubwayGameManager.Instance.standingCount * 0.5f;
        }
        else
        {
            stand.fillAmount = 0f;
        }
    }

    private void OnSlapButtonClicked()
    {
        StartCoroutine(ShowSlapCoolTime());
    }

    private void TutorialButtonBlocker()
    {
        if (TutorialManager.Instance.isSlapTutorial)
        {
            GetImage((int)Images.TutorialFallAsleepImage).fillAmount = 1f;
            GetImage((int)Images.TutorialStandingImage).fillAmount = 1f;

            for (int i = 0; i < (int)Buttons.MaxCount; i++)
            {
                GetButton(i).image.raycastTarget = false;

                if (i == (int)Buttons.SlapButton)
                {
                    GetImage((int)Images.TutorialSlapImage).fillAmount = 0f;
                    GetButton(i).image.raycastTarget = true;
                }
            }
            GetButton((int)Buttons.PauseButton).image.raycastTarget = true;
        }
        else if (TutorialManager.Instance.isStandingTutorial)
        {
            GetImage((int)Images.TutorialSlapImage).fillAmount = 1f;
            GetImage((int)Images.TutorialFallAsleepImage).fillAmount = 1f;

            for (int i = 0; i < (int)Buttons.MaxCount; i++)
            {
                GetButton(i).image.raycastTarget = false;

                if (i == (int)Buttons.StandingButton)
                {
                    GetImage((int)Images.TutorialStandingImage).fillAmount = 0f;
                    GetButton(i).image.raycastTarget = true;
                }
            }
            GetButton((int)Buttons.PauseButton).image.raycastTarget = true;
        }
        else if (TutorialManager.Instance.isSkipTutorial)
        {
            GetImage((int)Images.TutorialSlapImage).fillAmount = 1f;
            GetImage((int)Images.TutorialFallAsleepImage).fillAmount = 1f;

            for (int i = 0; i < (int)Buttons.MaxCount; i++)
            {
                GetButton(i).image.raycastTarget = false;
                if (i == (int)Buttons.StandingButton)
                {
                    GetImage((int)Images.TutorialStandingImage).fillAmount = 0f;
                    GetButton(i).image.raycastTarget = true;
                }
            }
            GetButton((int)Buttons.PauseButton).image.raycastTarget = true;
        }
        else
        {
            GetImage((int)Images.TutorialSlapImage).fillAmount = 0f;
            GetImage((int)Images.TutorialFallAsleepImage).fillAmount = 0f;
            GetImage((int)Images.TutorialStandingImage).fillAmount = 0f;
        }
    }
}
