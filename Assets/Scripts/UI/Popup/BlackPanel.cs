using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UI_FadeOutBlackPanel : UI_Popup
{
    [SerializeField] private float fadeDuration = 1f;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    public override void Init()
    {
        base.Init();
    }

    public void FadeIn(float duration = 1f) // 알파 1 -> 0
    {
        StartCoroutine(Fade(1f, 0f, duration, autoClose: true));
    }

    public void FadeOut(float duration = 1f) // 알파 0 → 1
    {
        StartCoroutine(Fade(0f, 1f, duration, autoClose: true));
    }

    private IEnumerator Fade(float from, float to, float duration, bool autoClose = false)
    {
        yield return null;

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        if (autoClose)
        {
            yield return new WaitForSeconds(0.25f); // 약간 대기 후 제거
            ClosePopupUI();
        }
    }

}
