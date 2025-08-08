using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_FadeBlackPanel : UI_Popup
{
    private bool isInited = false;
    CanvasGroup _canvasGroup;


    enum Images
    {
        BlackPanel,
    }

    void Start()
    {
        
    }

    private Image _blackPanel;

    public override void Init()
    {
        if (isInited) return;

        _canvasGroup = GetComponent<CanvasGroup>();
        // 처음엔 투명
        SetAlpha(0f);
        isInited = true;
    }

    private void SetAlpha(float alpha)
    {
        if (_canvasGroup != null)
        _canvasGroup.alpha = alpha;
    }

    //페이드 인 (검은 화면 보여주다가 닫기)
    public void StartFadeIn(float duration, float waitTime)
    {
        if (!isInited)
        {
            Init();
        }
        StopAllCoroutines();
        StartCoroutine(FadeAndDestroy(0f, 1f, duration, waitTime));
    }
    
    //페이드 아웃
    public void StartFadeOut(float duration)
    {
        if (!isInited)
        {
            Init();
        }
        StopAllCoroutines();
        StartCoroutine(FadeAndDestroy(1f, 0f, duration, 0f));
    }

    //페이드 인아웃 같이
    public void FadeInOut(float duration, float waitTime)
    {
        if (!isInited)
        {
            Init();
        }
        StartCoroutine(FadeRoutine(duration, waitTime));
    }
    
    //페이드 인+아웃 코루틴
    private IEnumerator FadeRoutine(float duration, float waitTime)
    {
        yield return StartCoroutine(FadeIn(duration));
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(FadeOut(duration));
        ClosePopupUI();
    }

    public IEnumerator FadeIn(float duration)
    {
        yield return StartCoroutine(Fade(0f, 1f, duration));
    }

    public IEnumerator FadeOut(float duration)
    {
        yield return StartCoroutine(Fade(1f, 0f, duration));
    }

    //페이드 인/ 아웃 따로
    private IEnumerator FadeAndDestroy(float from, float to, float duration, float waitTime)
    {
        yield return StartCoroutine(Fade(from, to, duration));

        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        ClosePopupUI();
    }

    //페이드 구현
    private IEnumerator Fade(float from, float to, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float alpha = Mathf.Lerp(from, to, timer / duration);
            SetAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        SetAlpha(to); // 마지막 값
    }
}