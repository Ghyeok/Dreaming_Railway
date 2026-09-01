using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using System.Collections;

public class UI_GameOverPopup : UI_Popup
{
    public GameObject blockerPanel;
    public CanvasGroup canvasGroup;
    private GameObject playerInputScript;
    [SerializeField] private float fadeInDuration = 1f;

    private bool isBGMOffBefore;

    private GameData _game;

    public enum Buttons
    {
        RetryButton,
        MainMenuButton,
        ExitButton,
    }

    public enum Texts
    {
        TimeText,
        StationText,
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

    void Awake()
    {
        var player = FindAnyObjectByType<Player>();
        if (player != null)
            playerInputScript = player.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Init()
    {
        base.Init();

        _game = GameDataManager.Instance.Game;

        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        GameObject retry = GetButton((int)Buttons.RetryButton).gameObject;
        AddUIEvent(retry, RetryButtonOnClicked);

        GameObject main = GetButton((int)Buttons.MainMenuButton).gameObject;
        AddUIEvent(main, MainMenuButtonOnClicked);

        GameObject exit = GetButton((int)Buttons.ExitButton).gameObject;
        AddUIEvent(exit, ExitButtonOnClicked);

        SoundManager.Instance.PlayAudioClip("GameOver", Sounds.SFX);

        ShowPlayTime();
        GameOverTutorial();
        GameDataManager.Instance.Timer.Pause();
    }

    private void RetryButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ClosePopupUI(this);
        ScriptManager.Instance.isStart = true;
        SceneTransitionManager.Instance.ExitFromDream();
    }

    private void MainMenuButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ClosePopupUI(this);
        SceneTransitionManager.Instance.GoToMain();
    }

    private void ExitButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.OnExitButton();
    }

    private void ShowPlayTime()
    {
        float playTime = GameDataManager.Instance.Timer.PlayTime;

        int min = Mathf.FloorToInt(playTime / 60);
        int sec = Mathf.FloorToInt(playTime % 60);
        int milSec = Mathf.FloorToInt((playTime * 100f) % 100);

        GetText((int)Texts.TimeText).text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, milSec);
    }

    private void GameOverTutorial()
    {
        if (_game.GameMode == GameMode.Tutorial)
        {
            TutorialManager.Instance.dialogState = TutorialManager.DialogState.Gameover;
            TutorialManager.Instance.isGameoverTutorial = true;
            TutorialManager.Instance.startIncreaseTired = false;

            if (DreamManager.Instance.isGameOverInDream)
            {
                TutorialManager.Instance.isDarkGameOverTutorial = true;
                TutorialManager.Instance.gameoverIdx = 0;
            }

            if (TutorialManager.Instance.tutorialPopup != null)
            {
                TutorialManager.Instance.tutorialPopup.gameObject.SetActive(true);
                TutorialManager.Instance.tutorialPopup.AdvanceDialog();
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

        if (playerInputScript != null)
        {
            //플레이어 움직임 비활성화
            playerInputScript.SetActive(false);
        }

        StartCoroutine(FadeInCoroutine(fadeInDuration));
    }

    private void OnDisable()
    {
        if (!isBGMOffBefore)
        {
            SoundManager.Instance.SetBGMOff(false);
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
