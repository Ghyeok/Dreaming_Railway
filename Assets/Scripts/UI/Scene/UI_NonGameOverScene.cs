using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_NonGameOverScene : UI_Scene
{
    public enum Buttons
    {
        LeftButton,
        RightButton,
        JumpButton,
        PauseButton,
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
        GameObject pause = GetButton((int)Buttons.PauseButton).gameObject;
        AddUIEvent(pause, PauseButtonOnClicked, Define.UIEvent.Click);
    }

    private void PauseButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_PausePopup");
        Debug.Log("일시정지!");
        Time.timeScale = 0;
    }
}
