using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public enum BackgroundType
    {
        Underground,
        Hangang,
        Station,
        Connecter,
    }

    public BackgroundType type;

    public RectTransform canvasRect;
    public RectTransform image1;
    public RectTransform image2;

    public float scrollSpeed;

    [SerializeField]
    private float leftX;
    private float height;

    [SerializeField]
    private float imageWidth;
    [SerializeField]
    private float screenWidth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasRect = gameObject.transform.parent.parent.GetComponent<RectTransform>();
        height = 200f;

        imageWidth = image1.rect.width;
        screenWidth = canvasRect.rect.width;
        leftX = imageWidth / 2 + screenWidth / 2;

        image1.anchoredPosition = new Vector2(imageWidth / 2 - screenWidth / 2, height);
        image2.anchoredPosition = image1.anchoredPosition + new Vector2(imageWidth, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(scrollSpeed == 0)
            return;

        float deltaX = scrollSpeed * Time.deltaTime;
        ScrollBackground(image1,deltaX);
        ScrollBackground(image2,deltaX);


        if(image1.anchoredPosition.x <= -leftX)
        {
            image1.anchoredPosition = image2.anchoredPosition + new Vector2(imageWidth, 0);
        }
        if (image2.anchoredPosition.x <= -leftX)
        {
            image2.anchoredPosition = image1.anchoredPosition + new Vector2(imageWidth, 0);
        }
    }

    private void ScrollBackground(RectTransform rect, float delta)
    {
        rect.anchoredPosition -= new Vector2(delta, 0);
    }
}
