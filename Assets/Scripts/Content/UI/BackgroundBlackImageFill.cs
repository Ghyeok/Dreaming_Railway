using UnityEngine;
using UnityEngine.UI;

public class BackgroundBlackImageFill : MonoBehaviour
{
    private Image image;
    private TirednessData _tiredness;

    private void Awake()
    {
        image = GetComponent<Image>();

        // 순수 C# 데이터 객체이므로 한 번만 캐싱한다 (종료 중 싱글톤 재접근 회피)
        _tiredness = GameDataManager.Instance.Tiredness;
    }

    private void OnEnable()
    {
        if (_tiredness == null) return;

        _tiredness.OnTiredChange += UpdateFill;
        UpdateFill(_tiredness.CurrentTiredness); // 초기값
    }

    private void OnDisable()
    {
        if (_tiredness == null) return;

        _tiredness.OnTiredChange -= UpdateFill;
    }

    void UpdateFill(float currentTired)
    {
        image.fillAmount = 1f - (currentTired / _tiredness.MaxTiredness);
    }
}
