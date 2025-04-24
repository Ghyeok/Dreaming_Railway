using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UI_Bind
{
    public enum Buttons
    {
        StandingButton, // 입석
        FallAsleepButton, // 즉시 잠들기
        SlapButton, // 뺨 떄리기
        GetOffButton, // 목적지에 내리기
        PauseButton, // 일시정지
        // 필요한 버튼 추가..
    }

    void PauseButtonTest(PointerEventData data)
    {
        Debug.Log("일시정지!");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bind<Button>(typeof(Buttons));

        GameObject pause = GetButton((int)Buttons.PauseButton).gameObject;
        AddUIEvent(pause, PauseButtonTest, Define.UIEvent.Click);

        GameObject stand = GetButton((int)Buttons.StandingButton).gameObject;
        AddUIEvent(stand, data => PlayerStanding.TriggerStanding(), Define.UIEvent.Click);

        GameObject getOff = GetButton((int)Buttons.GetOffButton).gameObject;
        AddUIEvent(getOff, TransferManager.Instance.SuccessGetOff, Define.UIEvent.Click);

        GameObject slap = GetButton((int)Buttons.SlapButton).gameObject;
        AddUIEvent(slap, data => PlayerSlap.TriggerSlap(), Define.UIEvent.Click);

        GameObject fallAsleep = GetButton((int)Buttons.FallAsleepButton).gameObject;
        AddUIEvent(fallAsleep, data => PlayerFallAsleep.TriggerFallAsleep(), Define.UIEvent.Click);
    }

    public GameObject BindButton(Buttons buttons)
    {
        return GetButton((int)buttons).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
