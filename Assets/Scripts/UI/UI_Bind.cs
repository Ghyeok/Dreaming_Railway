using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Bind : MonoBehaviour
{
    public enum Buttons
    {
        StandingButton, // 입석
        FallAsleepButton, // 즉시 잠들기
        SlapButton, // 뺨 떄리기
        TransferButton, // 환승하기
        GetOffButton, // 목적지에 내리기
        PauseButton, // 일시정지
        // 필요한 버튼 추가..
    }
    public enum Texts
    {
        DayText,
        TransferText,
        TimeText,
        // 필요한 텍스트 추가..
    }
    public enum Images
    {
        Image,
        // 필요한 이미지 추가..
    }
    public enum GameObjects
    {
        GameObject,
        // 필요한 게임오브젝트 추가..
    }

    void FallAsleepButtonTest(PointerEventData data)
    {
        Debug.Log("꿈 속 진입!");
        SceneManager.LoadScene("InDream_PlayerMove");
    }
    void SlapButtonTest(PointerEventData data)
    {
        Debug.Log("뺨 때리기!");
    }
    void PauseButtonTest(PointerEventData data)
    {
        Debug.Log("일시정지!");
    }
    void GetOffButtonTest(PointerEventData data)
    {
        Debug.Log("목적지 도착!");
    }
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));

        GameObject pause = GetButton((int)Buttons.PauseButton).gameObject;
        AddUIEvent(pause, PauseButtonTest, Define.UIEvent.Click);

        GameObject stand = GetButton((int)Buttons.StandingButton).gameObject;
        AddUIEvent(stand, data => PlayerStanding.TriggerStanding(), Define.UIEvent.Click);

        GameObject getOff = GetButton((int)Buttons.GetOffButton).gameObject;
        AddUIEvent(getOff, GetOffButtonTest, Define.UIEvent.Click);

        GameObject slap = GetButton((int)Buttons.SlapButton).gameObject;
        AddUIEvent(slap, SlapButtonTest, Define.UIEvent.Click);

        GameObject fallAsleep = GetButton((int)Buttons.FallAsleepButton).gameObject;
        AddUIEvent(fallAsleep, FallAsleepButtonTest, Define.UIEvent.Click);

        GameObject transfer = GetButton((int)Buttons.TransferButton).gameObject;
        AddUIEvent(transfer, TransferManager.Instance.SuccessTransfer, Define.UIEvent.Click);

        TextMeshPro dayText = GetText((int)Texts.DayText);
        TextMeshPro transferText = GetText((int)Texts.TransferText);
        TextMeshPro timeText = GetText((int)Texts.TimeText);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 유니티 최상위 클래스인 Object 배열로 저장
    // 예를 들어, typeof(Button)을 키로 해서 Button 타입의 오브젝트들을 배열로 저장할 수 있음
    Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    // 유니티 하이라키에 있는 <T> 컴포넌트를 가진 type형 오브젝트를 이름으로 자동 매핑한 후, objects[] 딕셔너리에 넣어준다
    // Enum 값과 유니티 하이라키에 있는 게임 오브젝트의 이름을 같게 하자
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type); 
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true); // <T>가 게임오브젝트
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true); // <T>가 컴포넌트

            if (objects[i] == null)
                Debug.Log("Failed to Bind");
        }
    }

    // objects[] 딕셔너리에 있는 오브젝트를 반환하는 함수
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }

    // 자주 사용하는 UI들의 Get<T> 함수 사용하기 쉽게 다시 선언
    protected TextMeshPro GetText(int idx) { return Get<TextMeshPro>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }


    // 자동으로 UI에 이벤트를 연결해주는 함수
    public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Util.GetOrAddComponet<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.onClickHandler -= action;
                evt.onClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.onDraghandler -= action;
                evt.onDraghandler += action;
                break;

                // 필요한 UI Event 추가...
        }
    }

}
