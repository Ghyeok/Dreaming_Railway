using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_FadeWhitePanel : UI_Popup
{
    private bool isInited = false;
    CanvasGroup _canvasGroup;
    private Coroutine fadeCoroutine;

    enum Images
    {
        WhitePanel,
    }

    public override void Init()
    {
        base.Init();

        if (isInited) return;

        _canvasGroup = GetComponent<CanvasGroup>();
        SetAlpha(0f);
        isInited = true;
    }

    public void SetAlpha(float alpha)
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = alpha;
    }

    public void StartFadeOut(float duration, float waitTime)
    {
        if (!isInited) Init();
        StartFadeCoroutine(FadeAndDestroy(0f, 1f, duration, waitTime));
    }

    public void StartFadeIn(float duration)
    {
        if (!isInited) Init();
        StartFadeCoroutine(FadeAndDestroy(1f, 0f, duration, 0f));
    }

    public void FadeInOut(float duration, float waitTime)
    {
        if (!isInited) Init();
        StartFadeCoroutine(FadeRoutine(duration, waitTime));
    }

    private void StartFadeCoroutine(IEnumerator routine)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(routine);
    }

    public IEnumerator FadeRoutine(float duration, float waitTime)
    {
        yield return StartCoroutine(FadeOut(duration));
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(FadeIn(duration));

        UIManager.Instance.ClosePopupUI(this);
    }

    public IEnumerator FadeOut(float duration)
    {
        yield return Fade(0f, 1f, duration);
    }

    public IEnumerator FadeIn(float duration)
    {
        yield return Fade(1f, 0f, duration);
    }

    public IEnumerator FadeAndDestroy(float from, float to, float duration, float waitTime)
    {
        yield return Fade(from, to, duration);

        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        UIManager.Instance.ClosePopupUI(this);
    }

    public IEnumerator Fade(float from, float to, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float alpha = Mathf.Lerp(from, to, timer / duration);
            SetAlpha(alpha);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        SetAlpha(to);
    }
}