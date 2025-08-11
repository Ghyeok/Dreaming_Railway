using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_FadeBlackPanel : UI_Popup
{
    private bool isInited = false;
    CanvasGroup _canvasGroup;
    private Coroutine fadeCoroutine;


    enum Images
    {
        BlackPanel,
    }

    void Start()
    {

    }

    public override void Init()
    {
        base.Init();

        if (isInited) return;

        _canvasGroup = GetComponent<CanvasGroup>();
        // 처음엔 투명
        SetAlpha(0f);
        isInited = true;
    }

    public void SetAlpha(float alpha)
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = alpha;
    }

    //페이드 아웃 (검은 화면 보여주다가 닫기)
    public void StartFadeOut(float duration, float waitTime)
    {
        if (!isInited)
        {
            Init();
        }
        StartFadeCoroutine(FadeAndDestroy(0f, 1f, duration, waitTime));
    }

    //페이드 인  (밝아지는거)
    public void StartFadeIn(float duration)
    {
        if (!isInited)
        {
            Init();
        }
        StartFadeCoroutine(FadeAndDestroy(1f, 0f, duration, 0f));
    }

    //페이드 인아웃 같이
    public void FadeInOut(float duration, float waitTime)
    {
        if (!isInited)
        {
            Init();
        }
        StartFadeCoroutine(FadeRoutine(duration, waitTime));
    }

    private void StartFadeCoroutine(IEnumerator routine)
    {
        // 이미 실행 중인 페이드 코루틴이 있으면 멈춤
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        // 새로운 코루틴 실행
        fadeCoroutine = StartCoroutine(routine);
    }

    //페이드 인+아웃 코루틴
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

    //페이드 인/ 아웃 따로
    public IEnumerator FadeAndDestroy(float from, float to, float duration, float waitTime)
    {
        yield return Fade(from, to, duration);

        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }
        UIManager.Instance.ClosePopupUI(this);
    }

    //페이드 구현
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

        SetAlpha(to); // 마지막 값
    }
}