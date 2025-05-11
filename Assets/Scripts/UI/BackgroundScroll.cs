using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField]
    private float scrollSpeed;
    [SerializeField]
    private BackgroundSpawner.BackgroundType backgroundType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetScrollSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        ScrollBackground();
    }

    private void SetScrollSpeed()
    {
        if(backgroundType == BackgroundSpawner.BackgroundType.UNDERGROUND)
        {
            scrollSpeed = 100f;
        }
        else if (backgroundType == BackgroundSpawner.BackgroundType.HANGANG)
        {
            scrollSpeed = 100f;
        }
    }

    private void ScrollBackground()
    {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
    }
}
