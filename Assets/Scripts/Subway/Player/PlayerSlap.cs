using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlap : MonoBehaviour
{
    public Button slapButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slapButton = GetComponent<Button>();
        slapButton.onClick.AddListener(Slap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Slap()
    {
        StartCoroutine(DecreaseTiredBySlap());
    }

    IEnumerator DecreaseTiredBySlap()
    {
        if (!SubwayManager.Instance.isSlapCoolTime && SubwayManager.Instance.playerState == SubwayManager.PlayerState.SLEEP)
        {
            SubwayManager.Instance.playerBehave = SubwayManager.PlayerBehave.SLAP;
            SubwayManager.Instance.isSlapCoolTime = true;
            SubwayManager.Instance.currentTired -= SubwayManager.Instance.tiredDecreaseBySlap;

            yield return new WaitForSeconds(5f); // 5초 쿨타임
            SubwayManager.Instance.playerBehave = SubwayManager.PlayerBehave.NONE;
            SubwayManager.Instance.isSlapCoolTime = false;
        }
    }
}
