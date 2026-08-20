using UnityEngine;
using UnityEngine.UI;

public class BackgroundBlackImageFill : MonoBehaviour
{
    private Image image;
    private TirednessData _tiredness;

    private void Awake()
    {
        image = GetComponent<Image>();
        _tiredness = GameDataManager.Instance.Tiredness;
    }

    private void OnEnable()
    {
        if (_tiredness == null) return;

        _tiredness.OnTiredChanged += UpdateFill;
        UpdateFill(_tiredness.CurrentTiredness); // 초기값
    }

    private void OnDisable()
    {
        if (_tiredness == null) return;

        _tiredness.OnTiredChanged -= UpdateFill;
    }

    void UpdateFill(float currentTired)
    {
        image.fillAmount = 1f - (currentTired / _tiredness.MaxTiredness);
    }
}
