using TMPro;
using UnityEngine;
using static UI_Button;

public class UI_Text : UI_Bind
{
    public enum Texts
    {
        DayText,
        TransferText,
        TimeText,
        TiredText,
        // 필요한 텍스트 추가..
    }

    public TextMeshProUGUI tiredText;
    public TextMeshProUGUI transferText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        TextMeshProUGUI dayText = GetText((int)Texts.DayText);
        transferText = GetText((int)Texts.TransferText);
        TextMeshProUGUI timeText = GetText((int)Texts.TimeText);
        tiredText = GetText((int)Texts.TiredText);
    }

    public GameObject BindText(Texts texts)
    {
        return GetText((int)texts).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        TiredText();
        TransferText();
    }

    void TiredText()
    {
        TiredManager tiredManager = TiredManager.Instance;
        tiredText.text = $"{tiredManager.currentTired} / {tiredManager.maxTired}"; 
    }

    void TransferText()
    {
        StationManager stationManager = StationManager.Instance;
        transferText.text = $"{stationManager.currentStationIdx} / {stationManager.transferStationIdx} Stations";
    }
}
