using UnityEngine;
using UnityEngine.UI;

public class OutsideScroller : MonoBehaviour
{
    [Header("참조")]
    [SerializeField]
    private RectTransform canvasRect;
    [SerializeField]
    private RectTransform image1;
    [SerializeField]
    private RectTransform image2;
    [SerializeField]
    private BackgroundManager bm;
    [SerializeField]
    private Sprite hangang;
    [SerializeField]
    private Sprite grass;

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
    [SerializeField]
    private BackgroundManager.BackgroundType? outsideTheme = null;

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

    private void Update()
    {
        if (GameManager.Instance.gameState != GameState.Subway) return;
        if (scrollSpeed == 0) return;

        float deltaX = scrollSpeed * Time.deltaTime;
        ScrollBackground(image1, deltaX);
        ScrollBackground(image2, deltaX);

        bool isOutside =
            bm.currentType == BackgroundManager.BackgroundType.ConnectR ||
            bm.currentType == BackgroundManager.BackgroundType.Hangang ||
            bm.currentType == BackgroundManager.BackgroundType.Grass ||
            bm.currentType == BackgroundManager.BackgroundType.ConnectL;

        if (isOutside && outsideTheme.HasValue)
        {
            var spr = (outsideTheme.Value == BackgroundManager.BackgroundType.Hangang) ? hangang : grass;
            var img1 = image1.GetComponent<Image>();
            var img2 = image2.GetComponent<Image>();
            if (img1.sprite != spr) img1.sprite = spr;
            if (img2.sprite != spr) img2.sprite = spr;
        }
        else
        {
            // 바깥 시퀀스가 아니면 지우기
            var img1 = image1.GetComponent<Image>();
            var img2 = image2.GetComponent<Image>();
            if (img1.sprite != null) img1.sprite = null;
            if (img2.sprite != null) img2.sprite = null;
        }

        if (image1.anchoredPosition.x <= -leftX)
        {
            image1.anchoredPosition = image2.anchoredPosition + new Vector2(imageWidth, 0);
            prevX1 = image1.anchoredPosition.x;

            //if (bm.currentType != BackgroundManager.BackgroundType.ConnectL)
            //    scrollSpeed = bm.SetScrollSpeed(bm.currentType);
        }

        if (image2.anchoredPosition.x <= -leftX)
        {
            image2.anchoredPosition = image1.anchoredPosition + new Vector2(imageWidth, 0);
            prevX2 = image2.anchoredPosition.x;

            //if (bm.currentType != BackgroundManager.BackgroundType.ConnectL)
            //    scrollSpeed = bm.SetScrollSpeed(bm.currentType);
        }

        prevX1 = image1.anchoredPosition.x;
        prevX2 = image2.anchoredPosition.x;
    }

    private void ScrollBackground(RectTransform rect, float delta)
    {
        rect.anchoredPosition -= new Vector2(delta, 0);
    }

    private void HandleNextBackgroundChange(BackgroundManager.BackgroundType current, BackgroundManager.BackgroundType? next)
    {
        if (current == BackgroundManager.BackgroundType.ConnectR && next.HasValue)
        {
            if (next.Value == BackgroundManager.BackgroundType.Hangang || next.Value == BackgroundManager.BackgroundType.Grass)
            {
                outsideTheme = next.Value;
                scrollSpeed = bm.SetScrollSpeed(next.Value);
            }
        }

        if (bm.currentType == BackgroundManager.BackgroundType.Underground ||
                 bm.currentType == BackgroundManager.BackgroundType.Station)
        {
            outsideTheme = null;
        }
    }

    private void OnEnable()
    {
        BackgroundScroller.OnAfterBackgroundChange += HandleNextBackgroundChange;
    }

    private void OnDisable()
    {
        BackgroundScroller.OnAfterBackgroundChange -= HandleNextBackgroundChange;
    }
}
