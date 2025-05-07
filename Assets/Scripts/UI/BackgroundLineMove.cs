using UnityEngine;
using UnityEngine.UI;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetMoveRatio();
        MoveLine();
    }

    private void SetMoveRatio()
    {
        moveRatio = TiredManager.Instance.currentTired / TiredManager.Instance.maxTired;
    }

    private void MoveLine()
    {
        transform.rotation = Quaternion.Slerp(originRotation, targetRotation, moveRatio);
    }
}
