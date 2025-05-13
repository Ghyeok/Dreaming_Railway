using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PausePopup : UI_Popup
{
    public enum Buttons
    {
        ResumeButton,

    }

    public enum Images
    {
        BackgroundImage,
        MenuImage,
    }

    public enum Texts
    {

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
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        GameObject backgroundImage = GetImage((int)Images.BackgroundImage).gameObject;
        GameObject menuImage = GetImage((int)Images.MenuImage).gameObject;
        AddUIEvent(menuImage, MenuImageDrag, Define.UIEvent.Drag);

        GameObject resumeButton = GetButton((int)Buttons.ResumeButton).gameObject;
        AddUIEvent(resumeButton, ResumeButtonOnclicked, Define.UIEvent.Click);
    }

    private void ResumeButtonOnclicked(PointerEventData data)
    {
        Debug.Log("게임 재개!");
        UIManager.Instance.ClosePopupUI(this);
        Time.timeScale = 1;
    }

    private void MenuImageDrag(PointerEventData data)
    {

    }
}
