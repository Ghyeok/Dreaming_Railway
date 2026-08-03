using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BackgroundLineMove : MonoBehaviour
{
    private float moveRatio;
    private Quaternion originRotation;
    private Quaternion targetRotation;

    private void Awake()
    {
        originRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
        targetRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
    }

    private void OnEnable()
    {
        GameDataManager.Instance.Tiredness.OnTiredChange += UpdateRotation;
        UpdateRotation(GameDataManager.Instance.Tiredness.CurrentTiredness);
    }

    private void OnDisable()
    {
        GameDataManager.Instance.Tiredness.OnTiredChange -= UpdateRotation;
    }

    private void UpdateRotation(float currentTired)
    {
        moveRatio = currentTired / GameDataManager.Instance.Tiredness.MaxTiredness;
        transform.rotation = Quaternion.Slerp(originRotation, targetRotation, moveRatio);
    }
}