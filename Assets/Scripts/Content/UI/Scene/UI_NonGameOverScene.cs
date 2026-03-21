using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_NonGameOverScene : UI_Scene
{
    [SerializeField]
    private Player dreamPlayer;

    public enum Buttons
    {
        LeftButton,
        RightButton,
        JumpButton,
        PauseButton,
    }

    private void Awake()
    {
        dreamPlayer = FindAnyObjectByType<Player>();
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
        
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        GameObject pause = GetButton((int)Buttons.PauseButton).gameObject;
        AddUIEvent(pause, PauseButtonOnClicked, UIEvent.Click);

        GameObject left = GetButton((int)Buttons.LeftButton).gameObject;
        AddUIEvent(left, LeftButtonOnClicked, UIEvent.PointerDown);
        AddUIEvent(left, StopMoveOnPointerUp, UIEvent.PointerUp);

        GameObject right = GetButton((int)Buttons.RightButton).gameObject;
        AddUIEvent(right, RightButtonOnClicked, UIEvent.PointerDown);
        AddUIEvent(right, StopMoveOnPointerUp, UIEvent.PointerUp);

        GameObject jump = GetButton((int)Buttons.JumpButton).gameObject;
        AddUIEvent(jump, JumpButtonOnClicked, UIEvent.PointerDown);
    }

    private void PauseButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_PausePopup");
        Time.timeScale = 0;
    }

    private void LeftButtonOnClicked(PointerEventData data)
    {
        dreamPlayer.StartMoveLeft();
    }

    private void RightButtonOnClicked(PointerEventData data)
    {
        dreamPlayer.StartMoveRight();
    }

    private void JumpButtonOnClicked(PointerEventData data)
    {
        dreamPlayer.Jump();
    }

    private void StopMoveOnPointerUp(PointerEventData data)
    {
        dreamPlayer.StopMove();
    }

}
