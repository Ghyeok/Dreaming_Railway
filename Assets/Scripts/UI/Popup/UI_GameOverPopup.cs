using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_GameOverPopup : UI_Popup
{
    public enum Buttons
    {
        RetryButton,
        MainMenuButton,
        ExitButton,

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        GameObject retry = GetButton((int)Buttons.RetryButton).gameObject;
        AddUIEvent(retry, RetryButtonOnClicked);

        GameObject main = GetButton((int)Buttons.MainMenuButton).gameObject;
        AddUIEvent(main, MainMenuButtonOnClicked);

        GameObject exit = GetButton((int)Buttons.ExitButton).gameObject;
        AddUIEvent(exit, ExitButtonOnClicked);

        SoundManager.Instance.SetBGMOff();
        SoundManager.Instance.SetSFXOff();
        SoundManager.Instance.PlayAudioClip("GameOver", Define.Sounds.SFX);
    }

    private void RetryButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ClosePopupUI(this);
        SubwayGameManager.Instance.ResetGame();
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
}
