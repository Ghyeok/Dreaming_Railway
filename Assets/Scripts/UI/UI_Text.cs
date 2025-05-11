using TMPro;
using UnityEngine;
using static UI_Button;

public class UI_Text : UI_Popup
{
    public enum Texts
    {
        TransferText,
        TimerText,
        SlapText,
        // 필요한 텍스트 추가..
    }

    public TextMeshProUGUI transferText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI slapText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();   
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        timeText = GetText((int)Texts.TimerText);
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
        transferText.text = $"환승까지 <size=300%>{stationManager.transferStationIdx - stationManager.currentStationIdx}</size>역";
    }

    void SlapText()
    {
        slapText.text = $"{SubwayPlayerManager.Instance.slapNum}";
    }
}
