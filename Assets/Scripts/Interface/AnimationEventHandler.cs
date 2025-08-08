using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    private UI_FadeBlackPanel _fadePanel;

    public void CallFadeIn()
    {
        var fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeBlackPanel>();
        fadePanel.StartFadeIn(0.5f, 0.5f);
    }

    public void CallFadeOut()
    {
        var fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeBlackPanel>();
        fadePanel.StartFadeOut(0.5f);
    }

    public void CallFadeInOut()
    {
        var fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeBlackPanel>();
        fadePanel.FadeInOut(0.5f, 0.3f);
    }
}
