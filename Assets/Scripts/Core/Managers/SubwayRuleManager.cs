using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/* 플레이어가 지하철에 있을 때의 전체적인 흐름을 관리하는 매니저이다.
 * 게임 모드, 뺨 치기 쿨타임, 뺨 치기 피로도 감소량, 현재 Day 몇인지 등등을 다룬다.
 */
public class SubwayRuleManager : MonoBehaviour, ITickable
{
    private SubwaySceneRefs _refs;

    public float SlapCoolTime { get; private set; } = 5f;
    public float TiredDecreaseBySlap { get; private set; } = 3f;
    public bool IsSlapCoolTime { private set; get; } = false;

    public int StandingCount {  get; private set; } = 0;
    public bool IsStandingCoolDown { private set; get; } = false;

    public bool IsGameOverInSubway {  private set; get; } = false;
    public event Action OnSubwayGameOver;

    public float CurrentSlapCooldown {  get; private set; } // UI 업데이트 용

    public void Init(SubwaySceneRefs refs)
    {
        _refs = refs;

        ResetSubwayManager();
        SetSubwayScene();

        if(TimerManager.Instance != null)
        {
            TimerManager.Instance.Register(this);
        }
    }

    public void ResetSubwayManager()
    {
        StandingCount = 0;
        IsStandingCoolDown = false;

        IsSlapCoolTime = false;
        SlapCoolTime = 5f;
        CurrentSlapCooldown = 0f;

        IsGameOverInSubway = false;

        TiredDecreaseBySlap = (GameManager.Instance.GameMode == GameMode.InfiniteMode) ? 4f : 3f;
    }

    /// <summary>
    /// 게임이 시작되고 지하철 씬에 진입하면 최초 1회 실행되는 세팅 함수
    /// </summary>
    public void SetSubwayScene()
    {
        _refs.transferManager.OnTransferSuccess += AddStandingCount;

        // 씬 진입 시 게임오버인지 체크 후 처리
        if (_refs.transferManager != null && _refs.transferManager.IsMissedTransferStation)
        {
            GameOverInSubway();
        }
    }

    public void Tick(float deltaTime)
    {
        if (IsGameOverInSubway) return;

        // 뺨 때리기 쿨타임 판정
        if (IsSlapCoolTime)
        {
            CurrentSlapCooldown -= deltaTime;

            if (CurrentSlapCooldown <= 0f)
            {
                IsSlapCoolTime = false;
                CurrentSlapCooldown = SlapCoolTime;
            }
        }

        // 게임 오버 판정
        if (_refs.transferManager != null && _refs.transferManager.IsMissedTransferStation)
        {
            GameOverInSubway();
        }
    }

    public void GameOverInSubway()
    {
        if (IsGameOverInSubway) return;

        IsGameOverInSubway = true;
        OnSubwayGameOver?.Invoke();
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_GameOverPopup2");
    }

    public void StartSlapCooldown()
    {
        IsSlapCoolTime = true;
        CurrentSlapCooldown = SlapCoolTime;
    }

    public void StartStandingCooldown()
    {
        IsStandingCoolDown = true;
        StandingCount = 0; // 입석 쿨타임 시작 시 정거장 카운트 0으로 초기화
    }

    public void AddStandingCount()
    {
        if (!IsStandingCoolDown) return;

        StandingCount++;

        if (StandingCount >= 2)
        {
            IsStandingCoolDown = false;
            StandingCount = 0;
        }
    }

    private void OnDestroy()
    {
        if (TimerManager.Instance != null)
        {
            TimerManager.Instance.Unregister(this);
        }

        if (_refs != null && _refs.transferManager != null)
        {
            _refs.transferManager.OnTransferSuccess -= AddStandingCount;
        }
    }
}