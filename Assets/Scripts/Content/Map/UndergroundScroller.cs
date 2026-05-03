using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static BackgroundManager;

public class UndergroundScroller : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform image1;
    [SerializeField] private RectTransform image2;
    [SerializeField] private BackgroundManager.BackgroundType image1Type;
    [SerializeField] private BackgroundManager.BackgroundType image2Type;
    [SerializeField] private BackgroundManager bm;

    [SerializeField] private float leftX;
    [SerializeField] private float rightX;
    private float prevX1, prevX2;
    private const float EPS = 0.0001f;
    public float scrollSpeed;

    private float height;
    [SerializeField] private float imageWidth;
    [SerializeField] private float screenWidth;

    private Image img1, img2;

    public static event Action OnUndergroundBgChange;
    public static event Action<BackgroundManager.BackgroundType, BackgroundManager.BackgroundType?> OnAfterUndergroundBgChange;

    [SerializeField] private bool hasExecutedThisFrame = false;
    [SerializeField] private bool stationLerpRunning = false;
    [SerializeField] private float waitTime = 1.5f;
    [SerializeField] private Coroutine accelRoutine;

    void Start()
    {
        bm = GetComponentInParent<BackgroundManager>();
        canvasRect = gameObject.transform.parent.parent.GetComponent<RectTransform>();
        height = 200f;

        imageWidth = image1.rect.width;
        screenWidth = canvasRect.rect.width;
        leftX = imageWidth / 2 + screenWidth / 2;
        rightX = imageWidth / 2 - screenWidth / 2;

        image1.anchoredPosition = new Vector2(imageWidth / 2 - screenWidth / 2, height);
        image2.anchoredPosition = image1.anchoredPosition + new Vector2(imageWidth, 0);

        image1Type = bm.currentType;
        image2Type = bm.currentType;

        prevX1 = image1.anchoredPosition.x;
        prevX2 = image2.anchoredPosition.x;

        img1 = image1.GetComponent<Image>();
        img2 = image2.GetComponent<Image>();

        scrollSpeed = bm.SetScrollSpeed(bm.currentType);
    }

    void Update()
    {
        float deltaX = scrollSpeed * Time.deltaTime;
        ScrollBackground(image1, deltaX);
        ScrollBackground(image2, deltaX);

        bool crossed1 = (prevX1 > -rightX + EPS) && (image1.anchoredPosition.x <= -rightX + EPS);
        bool crossed2 = (prevX2 > -rightX + EPS) && (image2.anchoredPosition.x <= -rightX + EPS);

        if (!hasExecutedThisFrame && (crossed1 || crossed2))
        {
            hasExecutedThisFrame = true;

            if (crossed1 && crossed2)
            {
                if (image1.anchoredPosition.x <= image2.anchoredPosition.x)
                    ProcessImage1();
                else
                    ProcessImage2();
            }
            else if (crossed1)
                ProcessImage1();
            else if (crossed2)
                ProcessImage2();
        }

        if (image1.anchoredPosition.x <= -leftX)
        {
            image1.anchoredPosition = image2.anchoredPosition + new Vector2(imageWidth, 0);
            prevX1 = image1.anchoredPosition.x;

            if (bm.currentType != BackgroundManager.BackgroundType.ConnectL)
                scrollSpeed = bm.SetScrollSpeed(bm.currentType);
        }

        if (image2.anchoredPosition.x <= -leftX)
        {
            image2.anchoredPosition = image1.anchoredPosition + new Vector2(imageWidth, 0);
            prevX2 = image2.anchoredPosition.x;

            if (bm.currentType != BackgroundManager.BackgroundType.ConnectL)
                scrollSpeed = bm.SetScrollSpeed(bm.currentType);
        }

        prevX1 = image1.anchoredPosition.x;
        prevX2 = image2.anchoredPosition.x;

        UpdateImageVisibility();
    }

    // 야외 시퀀스(Hangang/Grass) 중에는 이 스크롤러 이미지를 숨기고, 복귀 후 다시 활성화
    private void UpdateImageVisibility()
    {
        bool isOutsideMiddle = bm.currentType == BackgroundType.Hangang
                            || bm.currentType == BackgroundType.Grass;

        if (isOutsideMiddle)
        {
            img1.enabled = false;
            img2.enabled = false;
            return;
        }

        img1.enabled = (image1Type != BackgroundType.Hangang && image1Type != BackgroundType.Grass);
        img2.enabled = (image2Type != BackgroundType.Hangang && image2Type != BackgroundType.Grass);
    }

    private void ProcessImage1()
    {
        OnUndergroundBgChange?.Invoke();

        img2.sprite = bm.ReturnBackgroundImage();
        image2Type = bm.currentType;

        BackgroundManager.BackgroundType? next = bm.backgroundQueue.Count > 0 ? bm.backgroundQueue.Peek() : null;
        OnAfterUndergroundBgChange?.Invoke(bm.currentType, next);

        if (bm.currentType == BackgroundManager.BackgroundType.ConnectL)
            scrollSpeed = bm.SetScrollSpeed(bm.currentType);
    }

    private void ProcessImage2()
    {
        OnUndergroundBgChange?.Invoke();

        img1.sprite = bm.ReturnBackgroundImage();
        image1Type = bm.currentType;

        BackgroundManager.BackgroundType? next = bm.backgroundQueue.Count > 0 ? bm.backgroundQueue.Peek() : null;
        OnAfterUndergroundBgChange?.Invoke(bm.currentType, next);

        if (bm.currentType == BackgroundManager.BackgroundType.ConnectL)
            scrollSpeed = bm.SetScrollSpeed(bm.currentType);
    }

    private void LateUpdate()
    {
        hasExecutedThisFrame = false;
    }

    private void ForceShowStationBackground()
    {
        img1.sprite = bm.ReturnBackgroundImage(BackgroundType.Station);
        img2.sprite = bm.ReturnBackgroundImage(BackgroundType.Station);
        image1Type = BackgroundType.Station;
        image2Type = BackgroundType.Station;
    }

    private void DecelScrollSpeed()
    {
        if (accelRoutine == null && bm.currentType == BackgroundType.Station && !stationLerpRunning && !SubwayFlowManager.Instance.IsTransferRecently)
        {
            stationLerpRunning = true;
            float decelTime = SubwayFlowManager.Instance.GetRemainTimeToDepartNextStation();
            StartCoroutine(StationDecelRoutine(scrollSpeed, decelTime));
        }
    }

    private void AccelScrollSpeed()
    {
        if (bm.currentType == BackgroundType.Station && SubwayFlowManager.Instance.CurrentStationIdx == 0)
        {
            ForceShowStationBackground();
            float accelTime = SubwayFlowManager.Instance.GetCurrentStationStoppingTime();
            accelRoutine = StartCoroutine(StationAccelRoutine(bm.lastSpeedBeforeStation, accelTime));
        }
    }

    private IEnumerator StationDecelRoutine(float speed, float time)
    {
        yield return null;
        float lerptime = time - waitTime;

        if (lerptime <= 0)
        {
            float ratio = lerptime / SubwayFlowManager.Instance.GetCurrentStationStoppingTime();
            speed = Mathf.SmoothStep(speed, 0, ratio);
            StartCoroutine(LerpSpeed(speed, 0f, Time.timeScale));
        }
        else
        {
            StartCoroutine(LerpSpeed(speed, 0f, lerptime));
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator StationAccelRoutine(float speed, float time)
    {
        yield return new WaitForSeconds(waitTime);
        yield return LerpSpeed(0f, speed, time);

        stationLerpRunning = false;
        accelRoutine = null;
    }

    private IEnumerator LerpSpeed(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            scrollSpeed = to;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            scrollSpeed = Mathf.SmoothStep(from, to, k);
            yield return null;
        }

        scrollSpeed = to;
    }

    private void ScrollBackground(RectTransform rect, float delta)
    {
        rect.anchoredPosition -= new Vector2(delta, 0);
    }

    public void OnDisableScroller()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        OnUndergroundBgChange += DecelScrollSpeed;
        SubwayFlowManager.Instance.OnLineEnded += AccelScrollSpeed;
        SubwayData.OnSubwayGameOver += OnDisableScroller;
    }

    private void OnDisable()
    {
        OnUndergroundBgChange -= DecelScrollSpeed;
        SubwayFlowManager.Instance.OnLineEnded -= AccelScrollSpeed;
        SubwayData.OnSubwayGameOver -= OnDisableScroller;
    }
}
