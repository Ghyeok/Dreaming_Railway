using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SubwayLinePopup : UI_Popup
{
    private SubwaySceneRefs _refs;
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

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        //TransferManager.OnTransferSuccess -= UpdateLinePopup;
        //TransferManager.OnTransferSuccess += UpdateLinePopup;
    }

    private void OnDisable()
    {
        //TransferManager.OnTransferSuccess -= UpdateLinePopup;
    }

    private void UpdateLinePopup()
    {
        SetLineText();
    }

    private void SetLineText()
    {
        StationManager station = _refs.stationManager;

        int lastTwoLineIdx = SubwayFlowManager.Instance.currentLineIdx - 2;
        int lastLineIdx = SubwayFlowManager.Instance.currentLineIdx - 1;
        int curLineIdx = SubwayFlowManager.Instance.currentLineIdx;
        int nextLineIdx = SubwayFlowManager.Instance.currentLineIdx + 1;

        if (lastTwoLineIdx >= 0)
        {
            GetText((int)Texts.LastTwoLineText).text = $"{SubwayFlowManager.Instance.SubwayLines[lastTwoLineIdx].transferIdx + 1}역 이동";
        }
        else
        {
            GetText((int)Texts.LastTwoLineText).text = " ";
        }

        if (lastLineIdx >= 0)
        {
            GetText((int)Texts.LastLineText).text = $"{SubwayFlowManager.Instance.SubwayLines[lastLineIdx].transferIdx + 1}역 이동";
        }
        else
        {
            GetText((int)Texts.LastLineText).text = " ";
        }

        if (!DreamManager.Instance.isInDream)
        {
            GetText((int)Texts.CurrentLineText).text = $"앞으로 {SubwayFlowManager.Instance.SubwayLines[curLineIdx].transferIdx - SubwayFlowManager.Instance.currentStationIdx + 1}역 뒤 환승";
        }
        else
        {
            GetText((int)Texts.CurrentLineText).text = $"앞으로 ???역 뒤 환승";
        }

        if (nextLineIdx < SubwayFlowManager.Instance.SubwayLines.Count)
        {
            GetText((int)Texts.NextLineText).text = $"{SubwayFlowManager.Instance.SubwayLines[nextLineIdx].transferIdx + 1}역 뒤 환승";
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
