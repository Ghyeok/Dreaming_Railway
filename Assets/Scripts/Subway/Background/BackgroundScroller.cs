using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static BackgroundManager;

public class BackgroundScroller : MonoBehaviour
{
    [Header("참조")]
    [SerializeField]
    private RectTransform canvasRect;
    [SerializeField]
    private RectTransform image1;
    [SerializeField]
    private RectTransform image2;
    [SerializeField]
    private BackgroundManager.BackgroundType image1Type;
    [SerializeField]
    private BackgroundManager.BackgroundType image2Type;
    [SerializeField]
    private BackgroundManager bm;

    [Header("배경 스크롤에 필요한 변수값")]
    [SerializeField]
    private float leftX; // 오른쪽 화면 끝의 좌표, 여기에 도착한 순간 다음 배경을 결정하고 출력한다
    [SerializeField]
    private float rightX; // 오른쪽 화면 끝의 좌표, 여기에 도착한 순간 다음 배경을 결정하고 출력한다
    private float prevX1, prevX2;
    private const float EPS = 0.0001f; // 경계 떨림 방지용
    public float scrollSpeed;

    private float height;
    [SerializeField]
    private float imageWidth;
    [SerializeField]
    private float screenWidth;

    public static event Action OnBackgroundChange;
    public static event Action<BackgroundManager.BackgroundType, BackgroundManager.BackgroundType?> OnAfterBackgroundChange;

    [SerializeField]
    private bool hasExecutedThisFrame = false;
    [SerializeField]
    private bool stationLerpRunning = false;
    [SerializeField]
    private float waitTime = 1.5f;
    [SerializeField]
    private Coroutine accelRoutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        scrollSpeed = bm.SetScrollSpeed(bm.currentType);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameState.Subway) return;

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
                // 더 왼쪽에 있는 이미지 우선
                if (image1.anchoredPosition.x <= image2.anchoredPosition.x)
                {
                    ProcessImage1();
                }
                else
                {
                    ProcessImage2();
                }
            }
            else if (crossed1)
            {
                ProcessImage1();
            }
            else if (crossed2)
            {
                ProcessImage2();
            }
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

        ShowAndHideConnectorImage();
    }

    private void ShowAndHideConnectorImage()
    {
        bool isOutside = (bm.currentType == BackgroundType.Hangang || bm.currentType == BackgroundType.Grass);

        // 지하 -> 야외 커넥터
        if (isOutside)
        {
            if (image1Type == BackgroundType.ConnectR && image1.anchoredPosition.x <= -leftX)
            {
                image1.GetComponent<Image>().enabled = false;
            }
            image2.GetComponent<Image>().enabled = false;
        }
        if (isOutside)
        {
            if (image2Type == BackgroundType.ConnectR && image1.anchoredPosition.x <= -leftX)
            {
                image2.GetComponent<Image>().enabled = false;
            }
            image1.GetComponent<Image>().enabled = false;
        }

        // 야외 -> 지하 커넥터
        if (bm.currentType == BackgroundType.ConnectL)
        {
            if (image1Type == BackgroundType.ConnectL && image1.anchoredPosition.x <= leftX)
            {
                image1.GetComponent<Image>().enabled = true;
            }

            if (image2Type == BackgroundType.ConnectL && image2.anchoredPosition.x <= leftX)
            {
                image2.GetComponent<Image>().enabled = true;
            }
        }

        // 이미지 타입이 지하, 정차역이면 항상 켜져 있음
        if (image1Type == BackgroundType.Underground || image1Type == BackgroundType.Station)
        {
            image1.GetComponent<Image>().enabled = true;
        }
        if (image2Type == BackgroundType.Underground || image2Type == BackgroundType.Station)
        {
            image2.GetComponent<Image>().enabled = true;
        }
    }

    private void ProcessImage1()
    {
        OnBackgroundChange?.Invoke();

        image2.GetComponent<Image>().sprite = bm.ReturnBackgroundImage();
        image2Type = bm.currentType;

        BackgroundManager.BackgroundType? next = bm.backgroundQueue.Count > 0 ? bm.backgroundQueue.Peek() : null;
        OnAfterBackgroundChange?.Invoke(bm.currentType, next);

        if (bm.currentType == BackgroundManager.BackgroundType.ConnectL)
            scrollSpeed = bm.SetScrollSpeed(bm.currentType);
    }

    private void ProcessImage2()
    {
        OnBackgroundChange?.Invoke();

        image1.GetComponent<Image>().sprite = bm.ReturnBackgroundImage();
        image1Type = bm.currentType;

        BackgroundManager.BackgroundType? next = bm.backgroundQueue.Count > 0 ? bm.backgroundQueue.Peek() : null;
        OnAfterBackgroundChange?.Invoke(bm.currentType, next);

        if (bm.currentType == BackgroundManager.BackgroundType.ConnectL)
            scrollSpeed = bm.SetScrollSpeed(bm.currentType);
    }

    private void LateUpdate()
    {
        hasExecutedThisFrame = false;
    }

    private void ForceShowStationBackground()
    {
        image1.GetComponent<Image>().sprite = bm.ReturnBackgroundImage(BackgroundType.Station);
        image2.GetComponent<Image>().sprite = bm.ReturnBackgroundImage(BackgroundType.Station);
        image1Type = BackgroundType.Station;
        image2Type = BackgroundType.Station;
    }

    private void DecelScrollSpeed() // 정차역이면 배경 스크롤 속도를 감소시킴
    {
        if (accelRoutine == null && bm.currentType == BackgroundType.Station && !stationLerpRunning && !TransferManager.Instance.isTransferRecently)
        {
            stationLerpRunning = true;

            float decelTime = StationManager.Instance.GetRemainTimeToTransfer();
            Debug.Log($"감속 시간: {decelTime}");
            StartCoroutine(StationDecelRoutine(scrollSpeed, decelTime));
        }
    }

    private void AccelScrollSpeed() // 환승 후, 정차역이면 배경 스크롤 속도를 증가시킴
    {
        if (bm.currentType == BackgroundType.Station && StationManager.Instance.currentStationIdx == 0)
        {
            ForceShowStationBackground();
            float accelTime = StationManager.Instance.GetCurrentStationStoppingTime(); // 평균 6초에 걸쳐 가속
            Debug.Log($"가속 시간: {accelTime}");
            accelRoutine = StartCoroutine(StationAccelRoutine(bm.lastSpeedBeforeStation, accelTime));
        }
    }

    private IEnumerator StationDecelRoutine(float speed, float time)
    {
        float lerptime = time - waitTime;
        Debug.Log($"Lerp Time : {lerptime}");

        if (lerptime <= 0) // 1.5초안에 환승함
        {
            float ratio = (float)(lerptime / StationManager.Instance.GetCurrentStationStoppingTime());
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
        OnBackgroundChange += DecelScrollSpeed;
        TransferManager.OnTransferSuccess += AccelScrollSpeed;

        SubwayGameManager.OnSubwayGameOver += OnDisableScroller;
        TransferManager.OnGetOffSuccess += OnDisableScroller;
    }

    private void OnDisable()
    {
        OnBackgroundChange -= DecelScrollSpeed;
        TransferManager.OnTransferSuccess -= AccelScrollSpeed;

        SubwayGameManager.OnSubwayGameOver -= OnDisableScroller;
        TransferManager.OnGetOffSuccess -= OnDisableScroller;
    }
}
