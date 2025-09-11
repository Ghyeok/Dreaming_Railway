using NUnit.Framework.Constraints;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerStanding : MonoBehaviour
{
    public static event Action OnStandingButtonClicked;
    public static bool skipLock;

    private void SkipStations() // 현재 위치에서 환승역까지의 시간을 더하는 함수
    {
        if (skipLock) return;

        if (SubwayGameManager.Instance.isStandingCoolDown) return;

        if (GameManager.Instance.gameMode == GameManager.GameMode.Tutorial)
        {
            TutorialManager.Instance.isStandingTutorial = false;
            TutorialManager.Instance.tutorialPopup.gameObject.SetActive(true);
            TutorialManager.Instance.tutorialPopup.AdvanceDialog();

            if (!TutorialManager.Instance.isSkipTutorial)
            {
                TutorialManager.Instance.startIncreaseTired = true;
                TimerManager.Instance.StartTimer();
            }
        }

        skipLock = true;

        Animator anim = SubwayPlayerManager.Instance.subwayPlayer.gameObject.GetComponent<Animator>();
        anim.SetTrigger("isSkip");

        StationManager.Instance.passedStations += StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].transferIdx + 1;
        TransferManager.Instance.ForceTransferByStanding();
    }

    public static void TriggerStanding()
    {
        OnStandingButtonClicked?.Invoke();
    }

    private void OnEnable()
    {
        OnStandingButtonClicked += SkipStations;
        TransferManager.OnTransferSuccess -= ReleaseSkipLock;
        TransferManager.OnTransferSuccess += ReleaseSkipLock;
    }

    private void OnDisable()
    {
        OnStandingButtonClicked -= SkipStations;
        TransferManager.OnTransferSuccess -= ReleaseSkipLock;
    }

    private void ReleaseSkipLock()
    {
        skipLock = false;
    }
}
