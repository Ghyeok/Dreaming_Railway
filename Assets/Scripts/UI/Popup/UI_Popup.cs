using UnityEngine;

public class UI_Popup : UI_Bind
{
    public override void Init()
    {
        UIManager.Instance.SetCanvas(gameObject, true);
    }

    public virtual void ClosePopupUI()
    {
        UIManager.Instance.ClosePopupUI(this);
    }
}
