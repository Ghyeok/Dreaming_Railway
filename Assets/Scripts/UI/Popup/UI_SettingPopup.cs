using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SettingPopup : UI_Popup
{
    public Vector2 lastThumbPos;

    private bool isBGMOff;
    private bool isSFXOff;

    private int bgmVolume;
    private int sfxVolume;

    private Sprite bgmOnSprite, bgmOffSprite, sfxOnSprite, sfxOffSprite;

    public enum Images
    {
        BGMCenter,
        SFXCenter,
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

        GameObject exit = GetButton((int)Buttons.ExitButton).gameObject;
        AddUIEvent(exit, ExitButtonOnClicked);

        GameObject bgm = GetButton((int)Buttons.BGMButton).gameObject;
        AddUIEvent(bgm, BGMButtonOnClicked);

        GameObject sfx = GetButton((int)Buttons.SFXButton).gameObject;
        AddUIEvent(sfx, SFXButtonOnClicked);

        GameObject thumb = GetButton((int)Buttons.ThumbButton).gameObject;
        AddUIEvent(thumb, ThumbButtonOnDragBegin, Define.UIEvent.DragBegin);
        AddUIEvent(thumb, ThumbButtonOnDrag, Define.UIEvent.Drag);
        AddUIEvent(thumb, ThumbButtonOnDragEnd, Define.UIEvent.DragEnd);

        UpdateSoundButtons();
    }

    private void UpdateSoundButtons()
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
        GetText((int)Texts.BGMText).text = bgmVolume.ToString();
    }

    private void SetSFXText()
    {
        GetText((int)Texts.BGMText).text = sfxVolume.ToString();
    }

    private void ThumbButtonOnDragBegin(PointerEventData data)
    {
        lastThumbPos = data.position;
    }

    private void ThumbButtonOnDrag(PointerEventData data)
    {

    }

    private void ThumbButtonOnDragEnd(PointerEventData data)
    {

    }
}
