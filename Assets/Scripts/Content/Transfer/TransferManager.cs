using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferManager : MonoBehaviour
{
    private SubwaySceneRefs _refs;

    public int curTransferCount; // 현재 환승 횟수
    public int maxTransferCount; // 최대 환승 횟수

    public  event Action OnTransferSuccess; // 조건 체크 후 환승에 성공한 순간 Invoke
    public  event Action OnGetOffSuccess; // 도착(게임 클리어)에 성공한 순간 Invoke

    public bool isTransferRecently; // 최근에 환승했는지 체크

    public void Init(SubwaySceneRefs refs)
    {
        _refs = refs;
        DetermineMaxTransferCount();

        _refs.stationManager.OnLineEnded += HandleLineEnded;
    }

    private void DetermineMaxTransferCount()
    {
        if (GameManager.Instance.GameMode == GameMode.InfiniteMode)
        {
            maxTransferCount = 99;
            return;
        }

        // StageSelectManager의 데이터를 기반으로 설정
        int stage = StageSelectManager.Instance.currentStage;
        maxTransferCount = stage == 0 ? 0 : stage + 1;
    }

    // 노선이 끝났을 때 호출되는 함수
    private void HandleLineEnded()
    {
        int lineIdx = _refs.stationManager.currentLineIdx;
        var currentLine = _refs.stationManager.subwayLines[lineIdx];

        // 게임 오버 조건 체크: 노선이 끝났는데 꿈속이라면? -> 게임 오버
        if (DreamManager.Instance.isInDream)
        {
            SubwayGameManager.Instance.isGameOver = true;
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
        _refs.stationManager.currentLineIdx++;
        _refs.stationManager.ResetStationManager(); // 내부 인덱스 및 타이머 리셋

        PlayTransferAnimation(); // 플레이어 쪽에서 구독하는 방향
        OnTransferSuccess?.Invoke();

        Debug.Log($"환승 성공! 현재 환승 횟수: {curTransferCount}");
    }

    private void ProcessArrival()
    {
        // 세이브 데이터 갱신
        var stageMng = StageSelectManager.Instance;
        if (stageMng.currentStage > stageMng.maxClearStage)
        {
            stageMng.maxClearStage = stageMng.currentStage;
            PlayerPrefs.SetInt("MaxClearStage", stageMng.maxClearStage);
            PlayerPrefs.Save();
        }

        GameManager.Instance.ChangeGameState(GameState.DaySelect);
        OnGetOffSuccess?.Invoke();
        UIManager.Instance.ShowPopupUI<UI_GameClearPopup>("UI_GameClearPopup");
    }

    private void PlayTransferAnimation()
    {
        if (SceneManager.GetActiveScene().name == "SubwayScene")
        {
            var player = SubwayPlayerManager.Instance.subwayPlayer;
            if (player != null)
            {
                player.GetComponent<Animator>().SetTrigger("isTransfer");
            }
        }
    }

    // 플레이어가 강제로 환승을 시도할 때 (입석 기능)
    public void ForceTransferByStanding()
    {
        // 강제 환승 시에도 StationManager의 노선 종료 처리를 호출하거나 직접 인덱스 조정
        curTransferCount++;
        _refs.stationManager.currentLineIdx++;
        _refs.stationManager.ResetStationManager();

        SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.SLEEP;
        OnTransferSuccess?.Invoke();
    }

    private void OnDestroy()
    {
        if (_refs != null && _refs.stationManager != null)
        {
            _refs.stationManager.OnLineEnded -= HandleLineEnded;
        }
    }
}