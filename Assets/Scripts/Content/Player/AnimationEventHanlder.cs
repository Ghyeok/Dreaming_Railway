using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationHandler : MonoBehaviour
{
    public void CallFadeOut()
    {
        var fadePanel = UI_FadeBlackPanel.Instance;
        fadePanel?.FadeOut(0.5f);
    }
    public void CallFadeIn()
    {
        var fadePanel = UI_FadeBlackPanel.Instance;
        fadePanel?.FadeIn(0f);
    }

    public void CallFadeInOut()
    {
        var fadePanel = UI_FadeBlackPanel.Instance;
        fadePanel?.FadeOutThenIn(0.5f, 0.5f);
    }
}