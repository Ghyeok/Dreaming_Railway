using UnityEngine;
using System.Collections;
using System;

public class UI_FadeBlackPanel : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private Coroutine fadeCoroutine;

    public static UI_FadeBlackPanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _canvasGroup = GetComponent<CanvasGroup>();
        SetAlpha(0f);
    }

    public void FadeOut(float duration, Action onComplete = null) => PlayFade(0f, 1f, duration, onComplete);
    public void FadeIn(float duration, Action onComplete = null) => PlayFade(1f, 0f, duration, onComplete);

    public void FadeOutThenIn(float outDuration, float inDuration, Action duringCovered = null, Action onComplete = null)
    {
        FadeOut(outDuration, () =>
        {
            duringCovered?.Invoke();
            FadeIn(inDuration, onComplete);
        });
    }

    private IEnumerator Fade(float from, float to, float duration, Action onComplete = null)
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

        // 완전히 투명해지면 입력 허용
        if (Mathf.Approximately(to, 0f))
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = true;
        }

        fadeCoroutine = null;
        onComplete?.Invoke();
    }

    private void PlayFade(float from, float to, float duration, Action onComplete = null)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = false;
        fadeCoroutine = StartCoroutine(Fade(from, to, duration, onComplete));
    }

    private void SetAlpha(float alpha)
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = alpha;
    }
}