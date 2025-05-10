using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    public enum BackgroundType
    {
        UNDERGROUND,
        HANGANG,
    }

    private Queue<GameObject> backgroundQueue = new Queue<GameObject>();
    [SerializeField]
    private bool isSpecialBackgroundShown = false;

    public GameObject undergroundPrefab;
    public GameObject hangangPrefab;

    [SerializeField]
    private GameObject currentBackground;
    [SerializeField]
    private GameObject nextBackground;
    [SerializeField]
    private GameObject backgroundParent;

    [SerializeField]
    private float width;

    public RectTransform spawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        backgroundParent = transform.gameObject;

        EnqueueInitialBackgrounds();
        SpawnInitialBackgrounds();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBackground.GetComponent<RectTransform>().localPosition.x <= -width)
        {
            Destroy(currentBackground);
            currentBackground = nextBackground;

            EnqueueNextBackground();
            GameObject nextPrefab = backgroundQueue.Dequeue();
            nextBackground = Instantiate(nextPrefab, Vector3.zero, Quaternion.identity, backgroundParent.transform);

            width = currentBackground.GetComponent<RectTransform>().rect.width;
            Vector3 nextPos = currentBackground.GetComponent<RectTransform>().localPosition + new Vector3(width - 1f, 0, 0);
  
            RectTransform nextRT = nextBackground.GetComponent<RectTransform>();
            nextRT.localPosition = nextPos;
        }
    }

    private void EnqueueInitialBackgrounds()
    {
        backgroundQueue.Enqueue(undergroundPrefab);
        backgroundQueue.Enqueue(undergroundPrefab);
    }

    private void SpawnInitialBackgrounds()
    {
        GameObject prefab1 = backgroundQueue.Dequeue();
        GameObject prefab2 = backgroundQueue.Dequeue();

        currentBackground = Instantiate(prefab1, Vector3.zero, Quaternion.identity, backgroundParent.transform);
        RectTransform currentRT = currentBackground.GetComponent<RectTransform>();
        currentRT.localPosition = Vector3.zero;

        width = currentRT.rect.width;
        Vector3 nextPos = currentRT.localPosition + new Vector3(width - 1f, 0f, 0f);

        nextBackground = Instantiate(prefab2, Vector3.zero, Quaternion.identity, backgroundParent.transform);
        RectTransform nextRT = nextBackground.GetComponent<RectTransform>();
        nextRT.localPosition = nextPos;
    }

    private void EnqueueNextBackground()
    {
        float rand = Random.value;

        if (rand <= 0.85f) // 85% 확률로 지하 배경
        {
            backgroundQueue.Enqueue(undergroundPrefab);
        }
        else // 15% 확률로 한강 배경
        {
            if (!isSpecialBackgroundShown)
            {
                backgroundQueue.Enqueue(hangangPrefab);
                isSpecialBackgroundShown = true;
            }
            else
            {
                backgroundQueue.Enqueue(undergroundPrefab);
            }
        }
    }
}
