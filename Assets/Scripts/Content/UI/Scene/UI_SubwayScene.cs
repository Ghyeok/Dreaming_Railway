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
    [SerializeField] private SubwayPlayer subwayPlayer;

    private SubwayData _subway;

    private CanvasGroup _fallAsleepCg;
    private CanvasGroup _slapCg;
    private CanvasGroup _standingCg;
    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _transferText;
    private TextMeshProUGUI _nextTransferText;
    private TextMeshProUGUI _passedStationText;
    private TextMeshProUGUI _slapText;

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

    private void Awake()
    {
        // 순수 C# 데이터 객체이므로 한 번만 캐싱한다 (종료 중 싱글톤 재접근 회피)
        _subway = GameDataManager.Instance.Subway;
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

        // 초기 페인트는 Bind가 끝난 뒤여야 한다.
        // OnEnable은 Start보다 먼저 돌아 Get<T>()가 null이므로 여기서 한다.
        UpdateStationUI();
        UpdateSlapUI();
        ShowStandingCoolDown();
    }

    private void OnEnable()
    {
        if (subwayPlayer != null)
        {
            subwayPlayer.OnStood += HandleStood;
            subwayPlayer.OnSlapSuccessed += TriggerSlapCoolTimeUI;
            subwayPlayer.OnSlapSuccessed += UpdateSlapUI;
        }

        if (_subway != null)
        {
            _subway.OnStationCompleteDepart += UpdateStationUI;
            _subway.OnTimeUpdated += UpdateTimerUI;
            _subway.OnLineEnded += CheckAndBlockStandingOnLastLine;
            _subway.OnStandingCooldownChanged += ShowStandingCoolDown;
        }
    }

    private void OnDisable()
    {
        if (subwayPlayer != null)
        {
            subwayPlayer.OnStood -= HandleStood;
            subwayPlayer.OnSlapSuccessed -= TriggerSlapCoolTimeUI;
            subwayPlayer.OnSlapSuccessed -= UpdateSlapUI;
        }

        if (_subway != null)
        {
            _subway.OnStationCompleteDepart -= UpdateStationUI;
            _subway.OnTimeUpdated -= UpdateTimerUI;
            _subway.OnLineEnded -= CheckAndBlockStandingOnLastLine;
            _subway.OnStandingCooldownChanged -= ShowStandingCoolDown;
        }
    }

    private void UpdateStationUI()
    {
        if (_subway == null || !_subway.HasLines) return;

        int line = _subway.CurrentLineIdx;
        int totalLines = _subway.SubwayLines.Count;

        if (_transferText != null)
        {
            int remaining = Mathf.Max(0, _subway.SubwayLines[line].transferIdx - _subway.CurrentStationIdx + 1);
            _transferText.text = $"환승까지 <size=300%>{remaining}</size>역";
        }

        if (_nextTransferText != null)
        {
            if (line + 1 < totalLines)
            {
                int nextTransferIdx = _subway.SubwayLines[line + 1].transferIdx;
                _nextTransferText.text = $"다음 환승 <size=300%>{Mathf.Max(0, nextTransferIdx + 1)}</size>역";
            }
            else
            {
                _nextTransferText.text = string.Empty;
            }
        }

        if (_passedStationText != null)
            _passedStationText.text = $"지나온 역: {_subway.PassedStations}";
    }

    private void UpdateSlapUI()
    {
        if (_slapText == null || _subway == null) return;

        _slapText.text = $"{_subway.SlapNum}";
    }

    private void UpdateTimerUI(float currentTime)
    {
        if (_timerText != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            _timerText.text = time.ToString(@"mm\:ss\:ff");
        }
    }

    private void HandleStood()
    {
        // 플레이어 입석 버튼 -> 스킵 버튼
        // 나머지 버튼 비활성화
        // 실제 로직은 스킵 버튼 눌렀을 때
        _slapCg.blocksRaycasts = false;
        _fallAsleepCg.blocksRaycasts = false;
        GetImage((int)Images.SlapFadeImage).fillAmount = 1f;
        GetImage((int)Images.FallAsleepFadeImage).fillAmount = 1f;
        GetButton((int)Buttons.StandingButton).GetComponent<Image>().sprite = Resources.Load<Sprite>("Arts/UIs/Subway/Player/Button_Skip");

        GameObject stand = GetButton((int)Buttons.StandingButton).gameObject;
        ClearUIEvent(stand);
        AddUIEvent(stand, data => subwayPlayer.TrySkip(), UIEvent.Click);
    }

    private void CheckAndBlockStandingOnLastLine()
    {
        if (_subway.CurTransferCount >= _subway.MaxTransferCount - 1)
        {
            _standingCg.blocksRaycasts = false;
            GetImage((int)Images.StandingFadeImage).fillAmount = 1f;
        }
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
        if (GameDataManager.Instance.Game.GameMode == GameMode.Tutorial)
        {
            _standingCg.blocksRaycasts = false;

            yield return new WaitForSecondsRealtime(STANDING_TUTORIAL_DELAY);

            _standingCg.blocksRaycasts = true;

            GameDataManager.Instance.Tutorial.ConsumeCurrentStep();
            TutorialSystem.Instance.TutorialPopup.gameObject.SetActive(true);
            TutorialSystem.Instance.TutorialPopup.AdvanceDialog();
        }
    }

    private void TriggerSlapCoolTimeUI() { StartCoroutine(ShowSlapCoolTime()); }
    private IEnumerator ShowSlapCoolTime()
    {
        float coolTime = _subway.SlapCoolTime;
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
        if (stand == null || _subway == null) return; // Bind 이전에 이벤트가 들어올 수 있다

        if (_subway.IsStandingCoolDown)
        {
            stand.fillAmount = 1 - _subway.StandingCount * STANDING_COOLDOWN_STEP;
        }
        else
        {
            // 마지막 노선에서는 입석이 불가하므로 계속 채워둔다.
            // CurTransferCount는 0-index라 마지막 노선을 타는 동안의 값이 MaxTransferCount - 1이다.
            stand.fillAmount = (_subway.CurTransferCount >= _subway.MaxTransferCount - 1) ? 1f : 0f;
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
        if (GameDataManager.Instance.Tutorial.IsSlapStep)
            BlockAllButtonsExcept((int)Buttons.SlapButton, (int)Images.SlapFadeImage);
        else if (GameDataManager.Instance.Tutorial.IsStandingStep || GameDataManager.Instance.Tutorial.IsSkipStep)
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