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
        // 필요한 텍스트 추가..
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        TextMeshProUGUI dayText = GetText((int)Texts.DayText);
        TextMeshProUGUI transferText = GetText((int)Texts.TransferText);
        TextMeshProUGUI timeText = GetText((int)Texts.TimeText);
    }

    public GameObject BindText(Texts texts)
    {
        return GetText((int)texts).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
