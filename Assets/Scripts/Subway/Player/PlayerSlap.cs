using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlap : MonoBehaviour
{
    public static event Action OnSlapButtonClicked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
            Debug.Log("뺨 때리기!");
            SubwayPlayerManager.Instance.playerBehave = SubwayPlayerManager.PlayerBehave.SLAP;
            if (SubwayPlayerManager.Instance.playerBehave == SubwayPlayerManager.PlayerBehave.SLAP)
            {
                SoundManager.Instance.SetSFXVolume(0.1f);
                SoundManager.Instance.PlayAudioClip("slap", Define.Sounds.SFX);
            }

            SubwayGameManager.Instance.isSlapCoolTime = true;
            TiredManager.Instance.currentTired -= SubwayGameManager.Instance.tiredDecreaseBySlap;
            SubwayPlayerManager.Instance.slapNum++;

            yield return new WaitForSeconds(5f); // 5초 쿨타임
            SubwayPlayerManager.Instance.playerBehave = SubwayPlayerManager.PlayerBehave.NONE;
            SubwayGameManager.Instance.isSlapCoolTime = false;
        }
    }

    public static void TriggerSlap()
    {
        OnSlapButtonClicked?.Invoke();
    }

    private void OnEnable()
    {
        OnSlapButtonClicked += Slap;
    }

    private void OnDisable()
    {
        OnSlapButtonClicked -= Slap;
    }
}
