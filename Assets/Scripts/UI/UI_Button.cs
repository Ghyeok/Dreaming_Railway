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

    public GameObject pause;
    public GameObject stand;
    public GameObject getOff;
    public GameObject slap;
    public GameObject fallAsleep;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bind<Button>(typeof(Buttons));

        pause = GetButton((int)Buttons.PauseButton).gameObject;
        AddUIEvent(pause, PauseButtonTest, Define.UIEvent.Click);

        stand = GetButton((int)Buttons.StandingButton).gameObject;
        AddUIEvent(stand, data => PlayerStanding.TriggerStanding(), Define.UIEvent.Click);

        getOff = GetButton((int)Buttons.GetOffButton).gameObject;
        AddUIEvent(getOff, TransferManager.Instance.SuccessGetOff, Define.UIEvent.Click);

        slap = GetButton((int)Buttons.SlapButton).gameObject;
        AddUIEvent(slap, data => PlayerSlap.TriggerSlap(), Define.UIEvent.Click);

        fallAsleep = GetButton((int)Buttons.FallAsleepButton).gameObject;
        AddUIEvent(fallAsleep, data => PlayerFallAsleep.TriggerFallAsleep(), Define.UIEvent.Click);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
