using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
class RotatingButton
{
    public RectTransform rect;
    public float angle;
}

public class UI_PausePopup : UI_Popup
{
    public List<RectTransform> buttons;

    [SerializeField]
    List<RotatingButton> rotatingButtons = new List<RotatingButton>();

    private RectTransform menu;
    private Vector2 lastMousePos;
    private float angleStep = 36f; // 버튼 사이 각도
    private float menuRadius;

    public enum Buttons
    {
        HowToButton,
        StationButton,
        ResumeButton,
        SettingButton,
        ExitButton,
        ChooseButton,
    }

    public enum Images
    {
        BackgroundImage,
        MenuImage,
    }

    public enum Texts
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        menu = GetImage((int)Images.MenuImage).gameObject.GetComponent<RectTransform>();
        menuRadius = GetButton((int)Buttons.ChooseButton).transform.position.x;
        ArrangeButtons();

        GameObject menuImage = GetImage((int)Images.MenuImage).gameObject;
        AddUIEvent(menuImage, MenuImageOnDrag, Define.UIEvent.Drag);
        AddUIEvent(menuImage, MenuImageDragBegin, Define.UIEvent.DragBegin);
        AddUIEvent(menuImage, MenuImageDragEnd, Define.UIEvent.DragEnd);

        GameObject chooseButton = GetButton((int)Buttons.ChooseButton).gameObject;
        AddUIEvent(chooseButton, ChooseButtonOnClicked, Define.UIEvent.Click);

    }

    private void ArrangeButtons() // 버튼 초기 위치 설정, 배열 초기화
    {
        rotatingButtons.Clear();

        float startAngle = 72f;
        for (int i = 0; i < buttons.Count; i++)
        {
            rotatingButtons.Add(new RotatingButton { rect = buttons[i], angle = startAngle - i * angleStep });
        }

        RectTransform howto = GetButton((int)Buttons.HowToButton).GetComponent<RectTransform>();
        RectTransform station = GetButton((int)Buttons.StationButton).GetComponent<RectTransform>();
        RectTransform resume = GetButton((int)Buttons.ResumeButton).GetComponent<RectTransform>();
        RectTransform setting = GetButton((int)Buttons.SettingButton).GetComponent<RectTransform>();
        RectTransform exit = GetButton((int)Buttons.ExitButton).GetComponent<RectTransform>();
        RectTransform choose = GetButton((int)Buttons.ChooseButton).GetComponent<RectTransform>();

        howto.anchoredPosition = new Vector2(menuRadius * Mathf.Cos(angleStep * 2 * Mathf.Deg2Rad), menuRadius * Mathf.Sin(angleStep * 2 * Mathf.Deg2Rad));
        station.anchoredPosition = new Vector2(menuRadius * Mathf.Cos(angleStep * Mathf.Deg2Rad), menuRadius * Mathf.Sin(angleStep * Mathf.Deg2Rad));
        resume.anchoredPosition = new Vector2(menuRadius * Mathf.Cos(0f * Mathf.Deg2Rad), menuRadius * Mathf.Sin(0f * Mathf.Deg2Rad));
        setting.anchoredPosition = new Vector2(menuRadius * Mathf.Cos(angleStep * Mathf.Deg2Rad), menuRadius * Mathf.Sin(-angleStep * Mathf.Deg2Rad));
        exit.anchoredPosition = new Vector2(menuRadius * Mathf.Cos(angleStep * 2 * Mathf.Deg2Rad), menuRadius * Mathf.Sin(-angleStep * 2 * Mathf.Deg2Rad));
    }

    private void UpdateButtonsAngle() // 각 버튼의 현재 각도 계산
    {
        float visibleMin = -85f;
        float visibleMax = 85f;

        foreach (var button in rotatingButtons)
        {
            bool isVisible = (button.angle >= visibleMin && button.angle <= visibleMax);

            float rad = button.angle * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * menuRadius;
            button.rect.anchoredPosition = pos;
            button.rect.gameObject.SetActive(isVisible);

            TextMeshProUGUI text = button.rect.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.rectTransform.localEulerAngles = Vector3.zero;
            }
        }
    }

    private float NormalizeAngle(float angle) // 오른쪽 반원 기준으로 각도 정규화
    {
        angle %= 180f;
        if(angle < -90f)
        {
            angle += 180f;
        }
        if(angle > 90f)
        {
            angle -= 180f;
        }
        return angle;
    }

    #region 일시정지 팝업 드래그 이벤트
    private void MenuImageDragBegin(PointerEventData data) // 드래그 시작
    {
        lastMousePos = data.position; // 스크린 좌표
    }
    private void MenuImageOnDrag(PointerEventData data) // 드래그 중
    {
        Vector2 currentMousePos = data.position;
        Vector2 menuCenter = RectTransformUtility.WorldToScreenPoint(null, menu.position);

        float prevAngle = Mathf.Atan2(lastMousePos.y - menuCenter.y, lastMousePos.x - menuCenter.x) * Mathf.Rad2Deg;
        float curAngle = Mathf.Atan2(currentMousePos.y - menuCenter.y, currentMousePos.x - menuCenter.x) * Mathf.Rad2Deg;

        float deltaAngle = Mathf.DeltaAngle(prevAngle, curAngle);

        foreach (var button in rotatingButtons)
        {
            button.angle += deltaAngle;
            button.angle = NormalizeAngle(button.angle);
        }

        UpdateButtonsAngle();
        lastMousePos = currentMousePos;
    }
    private void MenuImageDragEnd(PointerEventData data) // 드래그 끝, 어떤 버튼이든 하나는 선택버튼에 위치해야 한다
    {    
        float minDist = float.MaxValue;
        float chooseAngle = 0f;

        foreach (var button in rotatingButtons) // 선택 버튼과 나머지 버튼 사이의 최소 거리 찾기
        {
            float angle = NormalizeAngle(button.angle);
            float dist = Mathf.Abs(angle);
            if (dist < minDist)
            {
                minDist = dist;
                chooseAngle = angle;
            }
        }

        foreach (var button in rotatingButtons) // 모든 버튼을 최소 거리만큼 회전
        {
            button.angle -= chooseAngle;
            button.angle = NormalizeAngle(button.angle);
        }
        UpdateButtonsAngle();
    }
    #endregion

    private void ChooseButtonOnClicked(PointerEventData data) // 선택 버튼과 가장 가까운 버튼의 클릭 이벤트 호출
    {
        RectTransform choose = GetButton((int)Buttons.ChooseButton).gameObject.GetComponent<RectTransform>();
        RectTransform target = null;
        float minDist = float.MaxValue;
        float tolerance = 10f;

        foreach (var button in buttons)
        {
            float dist = Vector2.Distance(button.anchoredPosition, choose.anchoredPosition);
            if (dist < minDist)
            {
                minDist = dist;
                if (dist < tolerance)
                {
                    target = button;
                }
            }
        }

        if (target != null)
        {
            if (target.gameObject == GetButton((int)Buttons.HowToButton).gameObject)
            {
                HowToButtonOnclicked(data);
            }

            if (target.gameObject == GetButton((int)Buttons.StationButton).gameObject)
            {
                SubwayLinesButtonOnClicked(data);
            }

            if (target.gameObject == GetButton((int)Buttons.ResumeButton).gameObject)
            {
                ResumeButtonOnclicked(data);
            }

            if (target.gameObject == GetButton((int)Buttons.SettingButton).gameObject)
            {
                SettingButtonOnClicked(data);
            }

            if (target.gameObject == GetButton((int)Buttons.ExitButton).gameObject)
            {
                MainMenuButtonOnclicked(data);
            }
        }
    }

    #region 실제로 동작하는 버튼 이벤트 함수들의 구현
    private void HowToButtonOnclicked(PointerEventData data)
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_HowToDoPopup");
    }

    private void SubwayLinesButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_SubwayLinePopup");
    }

    private void ResumeButtonOnclicked(PointerEventData data)
    {
        Time.timeScale = 1;
        UIManager.Instance.ClosePopupUI(this);
    }

    private void SettingButtonOnClicked(PointerEventData data)
    {
        if (!DreamManager.Instance.isInDream)
        {
            UIManager.Instance.ShowPopupUI<UI_Popup>("UI_SettingPopup");
        }
        else if (DreamManager.Instance.isInDream)
        {
            UIManager.Instance.ShowPopupUI<UI_Popup>("UI_DreamSettingPopup");
        }
    }

    private void MainMenuButtonOnclicked(PointerEventData data)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
        DreamManager.Instance.isInDream = false;
    }
    #endregion
}
