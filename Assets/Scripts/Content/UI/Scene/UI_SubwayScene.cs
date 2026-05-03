using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SubwayScene : UI_Scene
{
    private const float STANDING_COOLDOWN_STEP = 0.5f;
    private const float STANDING_TUTORIAL_DELAY = 2f;

    [SerializeField] private GameObject tirednessUI;
    [SerializeField] private SubwayPlayerContext subwayPlayer;
    [SerializeField] private Animator anim;

    private CanvasGroup _fallAsleepCg;
    private CanvasGroup _slapCg;
    private CanvasGroup _standingCg;
    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _transferText;
    private TextMeshProUGUI _nextTransferText;
    private TextMeshProUGUI _passedStationText;
    private TextMeshProUGUI _slapText;

    [SerializeField] private float _time;

    public enum Buttons
    {
        StandingButton,
        FallAsleepButton,
        SlapButton,
        PauseButton,
        MaxCount,
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
        FallAsleepFadeImage,
        SlapFadeImage,
        StandingFadeImage,
        PassedStationImage,
    }

    public enum Texts
    {
        TransferText,
        NextTransferText,
        TimerText,
        SlapText,
        PassedStationText,
    }

    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _fallAsleepCg = Util.GetOrAddComponent<CanvasGroup>(GetButton((int)Buttons.FallAsleepButton).gameObject);
        _slapCg = Util.GetOrAddComponent<CanvasGroup>(GetButton((int)Buttons.SlapButton).gameObject);
        _standingCg = Util.GetOrAddComponent<CanvasGroup>(GetButton((int)Buttons.StandingButton).gameObject);
        _timerText = GetText((int)Texts.TimerText);
        _transferText = GetText((int)Texts.TransferText);
        _nextTransferText = GetText((int)Texts.NextTransferText);
        _passedStationText = GetText((int)Texts.PassedStationText);
        _slapText = GetText((int)Texts.SlapText);

        AddUIEvent(GetButton((int)Buttons.SlapButton).gameObject, SlapButtonOnClicked, UIEvent.Click);
        AddUIEvent(GetButton((int)Buttons.FallAsleepButton).gameObject, FallAsleepButtonOnClicked, UIEvent.Click);
        AddUIEvent(GetButton((int)Buttons.PauseButton).gameObject, PauseButtonOnClicked, UIEvent.Click);
        AddUIEvent(GetButton((int)Buttons.StandingButton).gameObject, StandButtonOnClicked, UIEvent.Click);

        UpdateStationUI();
    }

    private void OnEnable()
    {
        if (subwayPlayer != null && SubwayFlowManager.Instance != null)
        {
            subwayPlayer.OnStateChanged += HandlePlayerStateChanged;
            subwayPlayer.OnSlapSuccessed += TriggerSlapCoolTimeUI;
            subwayPlayer.OnSlapSuccessed += UpdateSlapUI;
            SubwayFlowManager.Instance.OnStationCompleteDepart += UpdateStationUI;
            SubwayFlowManager.Instance.OnTimeUpdated += UpdateTimerUI;
        }
    }

    private void OnDisable()
    {
        if (subwayPlayer != null && SubwayFlowManager.Instance != null)
        {
            subwayPlayer.OnStateChanged -= HandlePlayerStateChanged;
            subwayPlayer.OnSlapSuccessed -= TriggerSlapCoolTimeUI;
            subwayPlayer.OnSlapSuccessed -= UpdateSlapUI;
            SubwayFlowManager.Instance.OnStationCompleteDepart -= UpdateStationUI;
            SubwayFlowManager.Instance.OnTimeUpdated -= UpdateTimerUI;
        }
    }

    private void UpdateStationUI()
    {
        var flowManager = SubwayFlowManager.Instance;
        if (flowManager == null) return;

        int line = flowManager.CurrentLineIdx;
        int totalLines = flowManager.SubwayLines.Count;

        if (_transferText != null)
        {
            int remaining = Mathf.Max(0, flowManager.SubwayLines[line].transferIdx - flowManager.CurrentStationIdx + 1);
            _transferText.text = $"환승까지 <size=300%>{remaining}</size>역";
        }

        if (_nextTransferText != null)
        {
            if (line + 1 < totalLines)
            {
                int nextTransferIdx = flowManager.SubwayLines[line + 1].transferIdx;
                _nextTransferText.text = $"다음 환승 <size=300%>{Mathf.Max(0, nextTransferIdx + 1)}</size>역";
            }
            else
            {
                _nextTransferText.text = string.Empty;
            }
        }

        if (_passedStationText != null)
            _passedStationText.text = $"지나온 역: {flowManager.PassedStations}";
    }

    private void UpdateSlapUI()
    {
        if (_slapText != null)
            _slapText.text = $"{subwayPlayer.Rule.SlapNum}";
    }

    private void UpdateTimerUI(float currentTime)
    {
        _time = currentTime;

        if (_timerText != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            _timerText.text = time.ToString(@"mm\:ss\:ff");
        }
    }

    private void HandlePlayerStateChanged(PlayerState newState)
    {
        if (newState != PlayerState.STANDING) return;

        _slapCg.blocksRaycasts = false;
        _fallAsleepCg.blocksRaycasts = false;
        GetImage((int)Images.SlapFadeImage).fillAmount = 1f;
        GetImage((int)Images.FallAsleepFadeImage).fillAmount = 1f;
        GetButton((int)Buttons.StandingButton).GetComponent<Image>().sprite = Resources.Load<Sprite>("Arts/UIs/Subway/Player/Button_Skip");

        GameObject stand = GetButton((int)Buttons.StandingButton).gameObject;
        ClearUIEvent(stand);
        AddUIEvent(stand, data => subwayPlayer.TryStand(), UIEvent.Click);
    }

    private void PauseButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_PausePopup");
        GameManager.Instance.StopGame();
    }

    private void SlapButtonOnClicked(PointerEventData data)
    {
        subwayPlayer.TrySlap();
    }

    private void FallAsleepButtonOnClicked(PointerEventData data)
    {
        subwayPlayer.TryFallAsleep();
    }

    private void StandButtonOnClicked(PointerEventData data)
    {
        subwayPlayer.TryStand();
    }

    IEnumerator StandingTutorial()
    {
        if (GameManager.Instance.GameMode == GameMode.Tutorial)
        {
            _standingCg.blocksRaycasts = false;

            yield return new WaitForSecondsRealtime(STANDING_TUTORIAL_DELAY);

            _standingCg.blocksRaycasts = true;

            TutorialManager.Instance.isStandingTutorial = false;
            TutorialManager.Instance.tutorialPopup.gameObject.SetActive(true);
            TutorialManager.Instance.tutorialPopup.AdvanceDialog();
        }
    }

    private void TriggerSlapCoolTimeUI() { StartCoroutine(ShowSlapCoolTime()); }
    private IEnumerator ShowSlapCoolTime()
    {
        float coolTime = subwayPlayer.Rule.SlapCoolTime;
        float startTime = Time.time;
        Image slap = GetImage((int)Images.SlapFadeImage);

        slap.fillAmount = 1f;

        while (Time.time < startTime + coolTime)
        {
            slap.fillAmount = 1f - (Time.time - startTime) / coolTime;
            yield return null;
        }

        slap.fillAmount = 0f;
    }

    private void ShowStandingCoolDown()
    {
        Image stand = GetImage((int)Images.StandingFadeImage);

        if (subwayPlayer.Rule.IsStandingCoolDown)
        {
            stand.fillAmount = 1 - subwayPlayer.Rule.StandingCount * STANDING_COOLDOWN_STEP;
        }
        else
        {
            var flow = SubwayFlowManager.Instance;
            stand.fillAmount = (flow.CurTransferCount == flow.MaxTransferCount) ? 1f : 0f;
        }
    }

    private void BlockAllButtonsExcept(int allowedButtonIdx, int clearFadeImageIdx)
    {
        GetImage((int)Images.SlapFadeImage).fillAmount = 1f;
        GetImage((int)Images.FallAsleepFadeImage).fillAmount = 1f;
        GetImage((int)Images.StandingFadeImage).fillAmount = 1f;
        GetImage(clearFadeImageIdx).fillAmount = 0f;

        for (int i = 0; i < (int)Buttons.MaxCount; i++)
            GetButton(i).image.raycastTarget = (i == allowedButtonIdx);
        GetButton((int)Buttons.PauseButton).image.raycastTarget = true;
    }

    private void TutorialButtonBlocker()
    {
        if (TutorialManager.Instance.isSlapTutorial)
            BlockAllButtonsExcept((int)Buttons.SlapButton, (int)Images.SlapFadeImage);
        else if (TutorialManager.Instance.isStandingTutorial || TutorialManager.Instance.isSkipTutorial)
            BlockAllButtonsExcept((int)Buttons.StandingButton, (int)Images.StandingFadeImage);
        else
        {
            GetImage((int)Images.SlapFadeImage).fillAmount = 0f;
            GetImage((int)Images.FallAsleepFadeImage).fillAmount = 0f;
            GetImage((int)Images.StandingFadeImage).fillAmount = 0f;
        }
    }

    private void HideTirednessUI()
    {
        tirednessUI.SetActive(false);
    }
}