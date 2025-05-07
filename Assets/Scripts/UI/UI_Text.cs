using TMPro;
using UnityEngine;
using static UI_Button;

public class UI_Text : UI_Bind
{
    public enum Texts
    {
        DayText,
        TransferText,
        TimerText,
        SlapText,
        // 필요한 텍스트 추가..
    }

    public TextMeshProUGUI transferText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI slapText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        timeText = GetText((int)Texts.TimerText);
        dayText = GetText((int)Texts.DayText);
        transferText = GetText((int)Texts.TransferText);
        slapText = GetText((int)Texts.SlapText);
    }

    public GameObject BindText(Texts texts)
    {
        return GetText((int)texts).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        TransferText();
        SlapText();
    }

    void TransferText()
    {
        StationManager stationManager = StationManager.Instance;
        transferText.text = $"{stationManager.currentStationIdx} / {stationManager.transferStationIdx} Stations";
    }

    void SlapText()
    {
        slapText.text = $"{SubwayPlayerManager.Instance.slapNum}";
    }
}
