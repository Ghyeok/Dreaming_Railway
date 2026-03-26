using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferManager : MonoBehaviour
{
    private SubwaySceneRefs _refs;

    public int curTransferCount; // 현재 환승 횟수
    public int MaxTransferCount { get; private set; } // 최대 환승 횟수

    public  event Action OnTransferSuccess; // 조건 체크 후 환승에 성공한 순간 Invoke
    public  event Action OnGetOffSuccess; // 도착(게임 클리어)에 성공한 순간 Invoke

    public bool isTransferRecently; // 최근에 환승했는지 체크
    public bool IsMissedTransferStation { get; private set; } = false; // 환승역을 놓쳤는지? (게임오버)

    public void Init(SubwaySceneRefs refs)
    {
        _refs = refs;
        DetermineMaxTransferCount();

        SubwayFlowManager.Instance.OnLineEnded += HandleLineEnded;
    }

    private void DetermineMaxTransferCount()
    {
        if (GameManager.Instance.GameMode == GameMode.InfiniteMode)
        {
            MaxTransferCount = 99;
            return;
        }

        // StageSelectManager의 데이터를 기반으로 설정
        int stage = StageSelectManager.Instance.currentStage;
        MaxTransferCount = stage == 0 ? 0 : stage + 1;
    }

    // 노선이 끝났을 때 호출되는 함수
    private void HandleLineEnded()
    {
        int lineIdx = SubwayFlowManager.Instance.currentLineIdx;
        var currentLine = SubwayFlowManager.Instance.SubwayLines[lineIdx];

        // 게임 오버 조건 체크: 노선이 끝났는데 꿈속이라면? -> 게임 오버
        if (DreamManager.Instance.isInDream)
        {
            IsMissedTransferStation = true; 
            Debug.Log("환승/도착 시점에 꿈을 꾸고 있어 게임오버!");
            return;
        }

        // 도착역(Destination)인지 환승역(Transfer)인지 판별
        if (currentLine.hasDestination)
        {
            ProcessArrival();
        }
        else
        {
            ProcessTransfer();
        }
    }

    private void ProcessTransfer()
    {
        curTransferCount++;
        isTransferRecently = true;

        // StationManager의 데이터 업데이트 (다음 노선으로)
        SubwayFlowManager.Instance.currentLineIdx++;
        SubwayFlowManager.Instance.ResetFlow(); // 내부 인덱스 및 타이머 리셋

        OnTransferSuccess?.Invoke();

        Debug.Log($"환승 성공! 현재 환승 횟수: {curTransferCount}");
    }

    private void ProcessArrival()
    {
        GameManager.Instance.UpdateMaxClearDay();
        OnGetOffSuccess?.Invoke();
        UIManager.Instance.ShowPopupUI<UI_GameClearPopup>("UI_GameClearPopup");
    }

    // 플레이어가 강제로 환승을 시도할 때 (입석 기능)
    public void ForceTransferByStanding()
    {
        // 강제 환승 시에도 StationManager의 노선 종료 처리를 호출하거나 직접 인덱스 조정
        curTransferCount++;
        SubwayFlowManager.Instance.currentLineIdx++;
        SubwayFlowManager.Instance.ResetFlow();

        //SubwayPlayerController.Instance.playerState = SubwayPlayerController.PlayerState.SLEEP;
        OnTransferSuccess?.Invoke();
    }

    private void OnDestroy()
    {
        if (_refs != null && _refs.stationManager != null)
        {
            SubwayFlowManager.Instance.OnLineEnded -= HandleLineEnded;
        }
    }
}