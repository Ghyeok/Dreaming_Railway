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
        if (!SubwayGameManager.Instance.isSlapCoolTime && SubwayPlayerManager.Instance.playerState == SubwayPlayerManager.PlayerState.SLEEP)
        {
            SubwayPlayerManager.Instance.playerBehave = SubwayPlayerManager.PlayerBehave.SLAP;
            SubwayGameManager.Instance.isSlapCoolTime = true;
            TiredManager.Instance.currentTired -= SubwayGameManager.Instance.tiredDecreaseBySlap;

            yield return new WaitForSeconds(5f); // 5초 쿨타임
            SubwayPlayerManager.Instance.playerBehave = SubwayPlayerManager.PlayerBehave.NONE;
            SubwayGameManager.Instance.isSlapCoolTime = false;
        }
    }
}
