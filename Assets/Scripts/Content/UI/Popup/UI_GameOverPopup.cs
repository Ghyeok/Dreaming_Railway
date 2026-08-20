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

    public override void Init()
    {
        base.Init();

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
        //SubwayRuleManager.Instance.isGameOver = false;
        ScriptManager.Instance.isStart = true;
        SceneTransitionManager.Instance.ExitFromDream();
    }

    private void MainMenuButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ClosePopupUI(this);
        //SubwayRuleManager.Instance.isGameOver = false;
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
        if (GameDataManager.Instance.Game.GameMode != GameMode.Tutorial) return;

        // 현재 도달 가능한 게임오버는 '꿈속 게임오버' 하나뿐이다.
        // 환승 실패 경로는 SubwayData.SetGameOver() 호출부가 없어 죽어 있으므로
        // (스펙 8장), 복구되면 여기서 GameOverKind.Passed로 분기하면 된다.
        TutorialSystem.Instance.EnterGameOverPhase(GameOverKind.Dark);
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
            SoundManager.Instance.SetBGMOff(false);
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
