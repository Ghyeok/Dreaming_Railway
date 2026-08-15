using UnityEngine;

public class BackgroundLineMove : MonoBehaviour
{
    private float moveRatio;
    private Quaternion originRotation;
    private Quaternion targetRotation;

    private TirednessData _tiredness;

    private void Awake()
    {
        originRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
        targetRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

        // 순수 C# 데이터 객체이므로 한 번만 캐싱한다 (종료 중 싱글톤 재접근 회피)
        _tiredness = GameDataManager.Instance.Tiredness;
    }

    private void OnEnable()
    {
        if (_tiredness == null) return;

        _tiredness.OnTiredChange += UpdateRotation;
        UpdateRotation(_tiredness.CurrentTiredness);
    }

    private void OnDisable()
    {
        if (_tiredness == null) return;

        _tiredness.OnTiredChange -= UpdateRotation;
    }

    private void UpdateRotation(float currentTired)
    {
        moveRatio = currentTired / _tiredness.MaxTiredness;
        transform.rotation = Quaternion.Slerp(originRotation, targetRotation, moveRatio);
    }
}
