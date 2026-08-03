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
        GameDataManager.Instance.Tiredness.OnTiredChange += UpdateFill;
        UpdateFill(GameDataManager.Instance.Tiredness.CurrentTiredness); // 초기값
    }
    private void OnDisable()
    {
        GameDataManager.Instance.Tiredness.OnTiredChange -= UpdateFill;
    }

    void UpdateFill(float currentTired)
    {
        image.fillAmount = 1f - (currentTired / GameDataManager.Instance.Tiredness.MaxTiredness);
    }
}
