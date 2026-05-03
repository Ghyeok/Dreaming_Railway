using UnityEngine;
using UnityEngine.UI;

public class BackgroundBlackImageFill : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        TirednessManager.OnTiredChange += UpdateFill;
        UpdateFill(TirednessManager.CurrentTired); // 초기값
    }
    private void OnDisable()
    {
        TirednessManager.OnTiredChange -= UpdateFill;
    }

    void UpdateFill(float currentTired)
    {
        image.fillAmount = 1f - (currentTired / TirednessManager.MaxTired);
    }
}
