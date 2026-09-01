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
        AddUIEvent(exit, ExitButtonOnClicked, UIEvent.Click);
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

    private void SetLineText()
    {
        SubwayData subway = GameDataManager.Instance.Subway;

        int lastTwoLineIdx = subway.CurrentLineIdx - 2;
        int lastLineIdx = subway.CurrentLineIdx - 1;
        int curLineIdx = subway.CurrentLineIdx;
        int nextLineIdx = subway.CurrentLineIdx + 1;

        if (lastTwoLineIdx >= 0)
        {
            GetText((int)Texts.LastTwoLineText).text = $"{subway.SubwayLines[lastTwoLineIdx].transferIdx + 1}역 이동";
        }
        else
        {
            GetText((int)Texts.LastTwoLineText).text = " ";
        }

        if (lastLineIdx >= 0)
        {
            GetText((int)Texts.LastLineText).text = $"{subway.SubwayLines[lastLineIdx].transferIdx + 1}역 이동";
        }
        else
        {
            GetText((int)Texts.LastLineText).text = " ";
        }

        if (!DreamManager.Instance.isInDream)
        {
            GetText((int)Texts.CurrentLineText).text = $"앞으로 {subway.SubwayLines[curLineIdx].transferIdx - subway.CurrentStationIdx + 1}역 뒤 환승";
        }
        else
        {
            GetText((int)Texts.CurrentLineText).text = $"앞으로 ???역 뒤 환승";
        }

        if (nextLineIdx < subway.SubwayLines.Count)
        {
            GetText((int)Texts.NextLineText).text = $"{subway.SubwayLines[nextLineIdx].transferIdx + 1}역 뒤 환승";
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
