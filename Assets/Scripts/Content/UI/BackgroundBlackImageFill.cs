using UnityEngine;
using UnityEngine.UI;

public class BackgroundBlackImageFill : MonoBehaviour
{
    private float moveRatio;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetMoveRatio();
        FillBlackImage();
    }

    private void SetMoveRatio()
    {
        moveRatio = TiredManager.Instance.currentTired / TiredManager.Instance.maxTired;
    }

    private void FillBlackImage()
    {
        image.fillAmount = (1 - moveRatio);
    }

}
