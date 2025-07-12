using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UI_GameOverPopup : UI_Popup
{
    public GameObject blockerPanel;
    public CanvasGroup canvasGroup;
    private Player playerInputScript;
    [SerializeField] private float fadeInDuration = 1f;

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
        playerInputScript = GameObject.Find("Player").GetComponent<Player>();
        if (blockerPanel != null)
        {
            blockerPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

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

        SoundManager.Instance.SetBGMOff();
        SoundManager.Instance.PlayAudioClip("GameOver", Define.Sounds.SFX);

        TimerManager.Instance.StopTimer();
        ShowPlayTime();
    }

    private void RetryButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ClosePopupUI(this);
        GameManager.Instance.ResetGame();
        SceneManager.LoadScene("TestSubwayScene");
    }

    private void MainMenuButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ClosePopupUI(this);
        SubwayGameManager.Instance.isGameOver = false;
        SceneManager.LoadScene("MainScene");
    }

    private void ExitButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.OnExitButton();
    }

    private void ShowPlayTime()
    {
        float curTime = TimerManager.Instance.curTime;

        int min = Mathf.FloorToInt(curTime / 60);
        int sec = Mathf.FloorToInt(curTime % 60);
        int milSec = Mathf.FloorToInt((curTime * 100f) % 100);

        GetText((int)Texts.TimeText).text = string.Format("{0:00}:{1:00}:{2:00}", min, sec, milSec);
    }

    void OnEnable()
    {
        // 블로커 패널 활성화
        if (blockerPanel != null)
        {
            blockerPanel.SetActive(true);
        }

        //플레이어 움직임 비활성화
        playerInputScript.enabled = false;
        playerInputScript.isInObstacle = false;
        
        StartCoroutine(FadeInCoroutine(fadeInDuration));


    }

    void OnDisable()
    {
        StopAllCoroutines();
        blockerPanel.SetActive(false);
        playerInputScript.enabled = true;
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
