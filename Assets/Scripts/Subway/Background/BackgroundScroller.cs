using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    /*
     * 남은 일 
     * 1. 커넥터 연결 - 차라리 커넥터 확률을 5퍼로 두고, 커넥터가 나오면 반드시 한강 혹은 풀이 따라 나오는 구조로?
     * 2. 정차 구간 속도 줄이기 - 환승까지 1정거장 남았을 때, 약 10초간 점점 느려지다 멈추고 다시 빨라지는 구조?
     * 
     * 
     */


    [SerializeField]
    private RectTransform canvasRect;
    [SerializeField]
    private RectTransform image1;
    [SerializeField]
    private RectTransform image2;
    [SerializeField]
    private BackgroundManager bm;

    private float prevX1, prevX2;
    private const float EPS = 0.0001f; // 경계 떨림 방지용

    public float scrollSpeed;

    public static event Action OnBackgroundChange;

    [SerializeField]
    private float leftX; // 오른쪽 화면 끝의 좌표, 여기에 도착한 순간 다음 배경을 결정하고 출력한다
    [SerializeField]
    private float rightX; // 오른쪽 화면 끝의 좌표, 여기에 도착한 순간 다음 배경을 결정하고 출력한다

    private float height;

    [SerializeField]
    private float imageWidth;
    [SerializeField]
    private float screenWidth;

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

        prevX1 = image1.anchoredPosition.x;
        prevX2 = image2.anchoredPosition.x;

        scrollSpeed = bm.SetScrollSpeed(bm.currentType);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.Subway) return;
        if (scrollSpeed == 0) return;

        float deltaX = scrollSpeed * Time.deltaTime;
        ScrollBackground(image1, deltaX);
        ScrollBackground(image2, deltaX);

        bool crossed1 = (prevX1 > -rightX + EPS) && (image1.anchoredPosition.x <= -rightX + EPS);
        if (crossed1)
        {
            OnBackgroundChange?.Invoke();
            image2.GetComponent<Image>().sprite = bm.ReturnBackgroundImage();
            scrollSpeed = bm.SetScrollSpeed(bm.currentType);
        }

        bool crossed2 = (prevX2 > -rightX + EPS) && (image2.anchoredPosition.x <= -rightX + EPS);
        if (crossed2)
        {
            OnBackgroundChange?.Invoke();
            image1.GetComponent<Image>().sprite = bm.ReturnBackgroundImage();
            scrollSpeed = bm.SetScrollSpeed(bm.currentType);
        }

        if (image1.anchoredPosition.x <= -leftX)
        {
            image1.anchoredPosition = image2.anchoredPosition + new Vector2(imageWidth, 0);
            prevX1 = image1.anchoredPosition.x;

        }
        if (image2.anchoredPosition.x <= -leftX)
        {
            image2.anchoredPosition = image1.anchoredPosition + new Vector2(imageWidth, 0);
            prevX2 = image2.anchoredPosition.x;

        }

        prevX1 = image1.anchoredPosition.x;
        prevX2 = image2.anchoredPosition.x;
    }

    private void ScrollBackground(RectTransform rect, float delta)
    {
        rect.anchoredPosition -= new Vector2(delta, 0);
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }
}
