using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SubwayLinePopup : UI_Popup
{
    public enum Texts
    {
        LastTwoLineText,
        LastLineText,
        CurrentLineText,
        NextLineText
    }

    public enum Buttons
    {
        ExitButton,
    }

    public override void Init()
    {
        base.Init();

        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        GameObject exit = GetButton((int)Buttons.ExitButton).gameObject;
        AddUIEvent(exit, ExitButtonOnClicked, Define.UIEvent.Click);
    }

    private void Awake()
    {
        Init();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetLineText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        TransferManager.OnTransferSuccess -= UpdateLinePopup;
        TransferManager.OnTransferSuccess += UpdateLinePopup;
    }

    private void OnDisable()
    {
        TransferManager.OnTransferSuccess -= UpdateLinePopup;
    }

    private void UpdateLinePopup()
    {
        SetLineText();
    }

    private void SetLineText()
    {
        StationManager station = StationManager.Instance;

        int lastTwoLineIdx = station.currentLineIdx - 2;
        int lastLineIdx = station.currentLineIdx - 1;
        int curLineIdx = station.currentLineIdx;
        int nextLineIdx = station.currentLineIdx + 1;

        if (lastTwoLineIdx >= 0)
        {
            GetText((int)Texts.LastTwoLineText).text = $"{station.subwayLines[lastTwoLineIdx].transferIdx}역 이동";
        }
        else
        {
            GetText((int)Texts.LastTwoLineText).text = " "; 
        }

        if (lastLineIdx >= 0)
        {
            GetText((int)Texts.LastLineText).text = $"{station.subwayLines[lastLineIdx].transferIdx}역 이동";
        }
        else
        {
            GetText((int)Texts.LastLineText).text = " ";
        }

        GetText((int)Texts.CurrentLineText).text = $"앞으로 {station.subwayLines[curLineIdx].transferIdx - station.currentStationIdx}역 뒤 환승";

        if (nextLineIdx < station.subwayLines.Count)
        {
            GetText((int)Texts.NextLineText).text = $"{station.subwayLines[nextLineIdx].transferIdx}역 뒤 환승";
        }
        else
        {
            GetText((int)Texts.NextLineText).text = " ";
        }
    }

    private void ExitButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ClosePopupUI(this);
    }
}
