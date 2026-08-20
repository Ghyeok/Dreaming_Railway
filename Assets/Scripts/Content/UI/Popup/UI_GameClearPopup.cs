using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class UI_GameClearPopup : UI_Popup
{
    public GameObject blockerPanel;
    public CanvasGroup canvasGroup;
    [SerializeField] private float fadeInDuration = 1f;

    private bool isBGMOffBefore;

    public enum Buttons
    {
        LobbyButton,
    }

    public enum Texts
    {
        TimeText,
        DayText,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();

        if (blockerPanel != null)
        {
            blockerPanel.SetActive(false);
        }
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GameObject lobby = GetButton((int)Buttons.LobbyButton).gameObject;
        AddUIEvent(lobby, LobbyButtonOnClicked);

        SoundManager.Instance.GameClearSFX();
        GameDataManager.Instance.Timer.Pause();

        ShowPlayTime();
        ShowDayText();
        GameClearTutorial();
        GameClearNormalMode();
    }

    private void LobbyButtonOnClicked(PointerEventData data)
    {
        SceneTransitionManager.Instance.GoToStageSelect();
    }

    private void ShowDayText()
    {
        GetText((int)Texts.DayText).text = "Day " + GameDataManager.Instance.Game.CurrentDay;
    }

    private void ShowPlayTime()
    {
        float playTime = GameDataManager.Instance.Timer.PlayTime;

        int min = Mathf.FloorToInt(playTime / 60);
        int sec = Mathf.FloorToInt(playTime % 60);
        int milSec = Mathf.FloorToInt((playTime * 100f) % 100);

        GetText((int)Texts.TimeText).text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, milSec);
    }

    private void GameClearTutorial()
    {
        if (GameDataManager.Instance.Game.GameMode == GameMode.Tutorial)
        {
            // 변경 전: TutorialManager.Instance.isSubwayTutorialEnd = false;
            // 새 팝업의 Init()이 AdvanceDialog()를 타도록 현재 단계를 소비 처리한다
            GameDataManager.Instance.Tutorial.ConsumeCurrentStep();
            UIManager.Instance.ShowPopupUI<UI_TutorialPopup>("UI_TutorialPopup");
        }
    }

    private void GameClearNormalMode()
    {
        if (GameDataManager.Instance.Game.GameMode == GameMode.NormalMode)
        {
            if (ScriptManager.Instance.HasClearDialog(GameDataManager.Instance.Game.CurrentDay))
            {
                ScriptManager.Instance.isClear = true;

                if (ScriptManager.Instance.scriptPopup != null)
                {
                    ScriptManager.Instance.scriptPopup.gameObject.SetActive(true);
                    ScriptManager.Instance.ShowDialog(GameDataManager.Instance.Game.CurrentDay);
                }
                else
                {
                    UIManager.Instance.ShowPopupUI<UI_ScriptPopup>("UI_ScriptPopup");
                }
            }
        }   
    }

    private void OnEnable()
    {
        isBGMOffBefore = PlayerPrefs.GetInt("BGM_MUTE") == 1;
        SoundManager.Instance.SetBGMOff(true);

        // 블로커 패널 활성화
        if (blockerPanel != null)
        {
            blockerPanel.SetActive(true);
        }
        StartCoroutine(FadeInCoroutine(fadeInDuration));
    }

    private void OnDisable()
    {
        if (!isBGMOffBefore)
        {

        }
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
