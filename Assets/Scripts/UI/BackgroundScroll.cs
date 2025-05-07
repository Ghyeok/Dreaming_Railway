using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    RectTransform rectTransform;
    public float scrollSpeed;
    public Transform backgroundB;
    private float width;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        scrollSpeed = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
        if(transform.position.x <= -width/2)
        {
            transform.position = new Vector3((backgroundB.position.x + width), rectTransform.rect.height, 0f);
        }
    }
}
