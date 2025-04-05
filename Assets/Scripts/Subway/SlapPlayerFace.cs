using UnityEngine;
using UnityEngine.UI;

public class SlapPlayerFace : MonoBehaviour
{
    public Button slapButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slapButton = GetComponent<Button>();
        slapButton.onClick.AddListener(DecreaseTired);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DecreaseTired()
    {
        SubwayManager.Instance.currentTired -= 10f; // 임시 코드
    }
}
