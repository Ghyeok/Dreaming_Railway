using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_HowToDoPopup : UI_Popup
{
    public enum Images
    {
        HowToDoSubwayImage,
        HowToDoDreamImage,
    }

    public enum Buttons
    {
        ExitButton,
        BeforeButton,
        NextButton,

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
        Bind<Image>(typeof(Images));

        GameObject exit = GetButton((int)Buttons.ExitButton).gameObject;
        AddUIEvent(exit, ExitButtonOnClicked, Define.UIEvent.Click);

        GameObject before = GetButton((int)Buttons.BeforeButton).gameObject;
        AddUIEvent(before, BeforeButtonOnClicked, Define.UIEvent.Click);

        GameObject next = GetButton((int)Buttons.NextButton).gameObject;
        AddUIEvent(next, NextButtonOnClicked, Define.UIEvent.Click);

        GetImage((int)Images.HowToDoDreamImage).gameObject.SetActive(false);
        GetImage((int)Images.HowToDoSubwayImage).gameObject.SetActive(true);
    }

    private void ExitButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ClosePopupUI(this);
    }

    private void BeforeButtonOnClicked(PointerEventData data)
    {
        GetImage((int)Images.HowToDoDreamImage).gameObject.SetActive(false);
        GetImage((int)Images.HowToDoDreamImage - 1).gameObject.SetActive(true);
    }

    private void NextButtonOnClicked(PointerEventData data)
    {
        GetImage((int)Images.HowToDoSubwayImage).gameObject.SetActive(false);
        GetImage((int)Images.HowToDoSubwayImage + 1).gameObject.SetActive(true);
    }
}
