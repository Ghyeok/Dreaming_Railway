using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SettingPopup : UI_Popup
{
    [SerializeField]
    private bool isBGMOff;
    [SerializeField]
    private bool isSFXOff;

    private Sprite bgmOnSprite, bgmOffSprite, sfxOnSprite, sfxOffSprite;

    [SerializeField]
    private float bgmRadius, sfxRadius;
    [SerializeField]
    private RectTransform thumbPos, bgmCenter, sfxCenter, bgmBarStart, bgmBarEnd;

    public enum GameObjects
    {
        BGMCenter,
        SFXCenter,
        BGMBarStart,
        BGMBarEnd,
    }

    public enum Images
    {
        BGMBar,
    }

    public enum Buttons
    {
        ExitButton,
        BGMButton,
        SFXButton,
        ThumbButton,
    }

    public enum Texts
    {
        BGMText,
        SFXText,
    }

    private void Start()
    {
        bgmOnSprite = Resources.Load<Sprite>("Arts/UIs/Subway/Pause/Settings/button_bgm");
        bgmOffSprite = Resources.Load<Sprite>("Arts/UIs/Subway/Pause/Settings/button_bgm_off");
        sfxOnSprite = Resources.Load<Sprite>("Arts/UIs/Subway/Pause/Settings/button_sfx");
        sfxOffSprite = Resources.Load<Sprite>("Arts/UIs/Subway/Pause/Settings/button_sfx_off");

        Init();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        GameObject exit = GetButton((int)Buttons.ExitButton).gameObject;
        AddUIEvent(exit, ExitButtonOnClicked);

        GameObject bgm = GetButton((int)Buttons.BGMButton).gameObject;
        AddUIEvent(bgm, BGMButtonOnClicked);

        GameObject sfx = GetButton((int)Buttons.SFXButton).gameObject;
        AddUIEvent(sfx, SFXButtonOnClicked);

        GameObject thumb = GetButton((int)Buttons.ThumbButton).gameObject;
        AddUIEvent(thumb, BGMThumbButtonDragBegin, Define.UIEvent.DragBegin);
        AddUIEvent(thumb, BGMThumbButtonOnDrag, Define.UIEvent.Drag);

        UpdateSoundButtons();

        thumbPos = GetButton((int)Buttons.ThumbButton).GetComponent<RectTransform>();
        bgmCenter = Get<GameObject>((int)GameObjects.BGMCenter).GetComponent<RectTransform>();
        sfxCenter = Get<GameObject>((int)GameObjects.SFXCenter).GetComponent<RectTransform>();
        bgmBarStart = Get<GameObject>((int)GameObjects.BGMBarStart).GetComponent<RectTransform>();
        bgmBarEnd = Get<GameObject>((int)GameObjects.BGMBarEnd).GetComponent<RectTransform>();

        bgmRadius = Vector2.Distance(RectTransformUtility.WorldToScreenPoint(null, thumbPos.position),
                                     RectTransformUtility.WorldToScreenPoint(null, bgmCenter.position));

        UpdateSoundButtons();
        SetThumbPositionByVolume(SoundManager.Instance.bgmVolume);
    }

    private void UpdateSoundButtons() // 팝업 창을 껐다 켜도 버튼 상태 유지
    {
        isBGMOff = SoundManager.Instance.IsBGMOff;
        isSFXOff = SoundManager.Instance.IsSFXOff;

        Button bgm = GetButton((int)Buttons.BGMButton);
        bgm.image.sprite = isBGMOff ? bgmOffSprite : bgmOnSprite;

        Button sfx = GetButton((int)Buttons.SFXButton);
        sfx.image.sprite = isSFXOff ? sfxOffSprite : sfxOnSprite;
    }

    private void ExitButtonOnClicked(PointerEventData data)
    {
        UIManager.Instance.ClosePopupUI(this);
    }

    private void BGMButtonOnClicked(PointerEventData data)
    {
        if (!SoundManager.Instance.IsBGMOff)
        {
            SoundManager.Instance.SetBGMOff();
        }
        else
        {
            SoundManager.Instance.SetBGMOn();
        }

        UpdateSoundButtons();
    }

    private void SFXButtonOnClicked(PointerEventData data)
    {
        if (!SoundManager.Instance.IsSFXOff)
        {
            SoundManager.Instance.SetSFXOff();
        }
        else
        {
            SoundManager.Instance.SetSFXOn();
        }

        UpdateSoundButtons();
    }

    private void SetBGMText()
    {
        GetText((int)Texts.BGMText).text = Mathf.RoundToInt((SoundManager.Instance.bgmVolume * 100f)).ToString();
    }

    private void SetSFXText()
    {
        GetText((int)Texts.BGMText).text = (SoundManager.Instance.sfxVolume * 100f).ToString();
    }

    #region 원형 슬라이더 UI 구현

    private void BGMThumbButtonDragBegin(PointerEventData data)
    {

    }

    private void BGMThumbButtonOnDrag(PointerEventData data)
    {
        UpdateBGMThumbPosition(data);
    }

    private struct ArcInfo // 각도 정보 저장 구조체
    {
        public float angleStart; // 시작 각도
        public float angleEnd; // 끝 각도
        public float totalArc; // 시작 ~ 끝 각도 범위
    }

    private ArcInfo GetAngleArcInfo() // 기준이 되는 각도 계산
    {
        Vector2 screenCenter = RectTransformUtility.WorldToScreenPoint(null, bgmCenter.position);
        Vector2 screenStart = RectTransformUtility.WorldToScreenPoint(null, bgmBarStart.position);
        Vector2 screenEnd = RectTransformUtility.WorldToScreenPoint(null, bgmBarEnd.position);

        Vector2 startDir = (screenStart - screenCenter).normalized;
        Vector2 endDir = (screenEnd - screenCenter).normalized;

        float angleStart = GetAngle(startDir);
        float angleEnd = GetAngle(endDir);
        float totalArc = (angleStart - angleEnd + 360f) % 360f;

        return new ArcInfo { angleStart = angleStart, angleEnd = angleEnd, totalArc = totalArc };
    }

    private float GetAngle(Vector2 screenDir) // 현재 방향 각도 계산
    {
        return (Mathf.Atan2(screenDir.y, screenDir.x) * Mathf.Rad2Deg + 360f) % 360f;
    }

    private Vector3 GetThumbWorldPosition(float angleDeg) // 현재 각도에 따른 ThumbButton의 월드 좌표 계산
    {
        Vector2 center = RectTransformUtility.WorldToScreenPoint(null, bgmCenter.position);
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 screenPos = center + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * bgmRadius;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(thumbPos.parent as RectTransform, screenPos, null, out Vector3 worldPos);
        return worldPos;
    }

    private void ApplyBGMVolume(float fill) // fill값을 통한 ThumbButton, Bar, Volume 설정
    {
        float clampedFill = Mathf.Clamp01(fill);
        GetImage((int)Images.BGMBar).fillAmount = 1f - clampedFill;
        SoundManager.Instance.bgmVolume = SoundManager.Instance.SetBGMVolume(1f - clampedFill);
        SetBGMText();

        ArcInfo arc = GetAngleArcInfo();
        float angle = (arc.angleStart - arc.totalArc * clampedFill + 360f) % 360f;
        thumbPos.position = GetThumbWorldPosition(angle);
    }

    private void SetThumbPositionByVolume(float volume) // 팝업 창을 껐다 켜도 볼륨 상태 유지
    {
        ApplyBGMVolume(1 - volume);
    }

    private void UpdateBGMThumbPosition(PointerEventData data) // ThumbButton 위치 실시간 업데이트
    {
        Vector2 screenCenter = RectTransformUtility.WorldToScreenPoint(null, bgmCenter.position);
        Vector2 thumb = data.position;
        Vector2 dir = (thumb - screenCenter).normalized;

        ArcInfo arc = GetAngleArcInfo();
        float angle = GetAngle(dir);
        float curArc = (arc.angleStart - angle + 360f) % 360f;

        if (curArc > arc.totalArc)
            return;

        float fill = curArc / arc.totalArc;
        ApplyBGMVolume(fill);
    }
    #endregion
}
