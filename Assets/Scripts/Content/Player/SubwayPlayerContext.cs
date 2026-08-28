using System;
using UnityEngine;

public enum PlayerState
{
    NONE,
    SLEEP,
    STANDING,
    DEEPSLEEP,
}

public class SubwayPlayerContext : MonoBehaviour
{
    private TirednessSystem _tirednessManager;
    private TirednessData _tiredness;
    private GameData _game;
    public SubwayData Data { get; private set; }
    public Animator Anim { get; private set; }

    public event Action<PlayerState> OnStateChanged;
    public event Action OnSlapSuccessed;
    public event Action OnSkipped;

    public PlayerState CurrentState { get; private set; } = PlayerState.NONE;

    public void Init(TirednessSystem tirednessManager)
    {
        _tirednessManager = tirednessManager;
        _tiredness = GameDataManager.Instance.Tiredness;
        _game  = GameDataManager.Instance.Game;

        Data = GameDataManager.Instance.Subway;
        Data.ResetPlayerSession(_game.GameMode == GameMode.InfiniteMode);
        Anim = GetComponent<Animator>();

        Data.OnLineEnded -= Data.AddStandingCount;
        Data.OnLineEnded += Data.AddStandingCount;

        ChangeState(PlayerState.SLEEP);
        _tiredness.OnTiredChange += HandleTiredChange;
    }

    private void Update()
    {
        if (TimerManager.Instance == null || TimerManager.Instance.IsPaused) return;

        Data.TickSlapCooldown(Time.deltaTime);
    }

    public void ForceMaxTiredness() => _tirednessManager.SetTirednessForced(99.9f);

    private void ChangeState(PlayerState newState)
    {
        CurrentState = newState;

        if (newState == PlayerState.STANDING)
        {
            SoundManager.Instance.StandingSFX();
            Anim.SetTrigger("isStanding");
            ForceMaxTiredness();
        }

        OnStateChanged?.Invoke(CurrentState);
    }

    public void HandleTiredChange(float currentTired)
    {
        if (CurrentState == PlayerState.SLEEP)
            Anim.SetBool("isSleeping", _tiredness.IsTiredHalf);
    }

    public void TryTransfer()
    {
        Anim.SetTrigger("isTransfer");
    }

    public void TrySlap()
    {
        if (CurrentState == PlayerState.NONE || Data.IsSlapCoolTime) return;

        Data.StartSlapCooldown();
        SoundManager.Instance.SlapSFX();
        Anim.SetTrigger("isSlap");
        _tirednessManager.DecreaseTiredness(Data.TiredDecreaseBySlap);
        OnSlapSuccessed?.Invoke();
    }

    public void TryStand()
    {
        if (Data.IsStandingCoolDown) return;
        ChangeState(PlayerState.STANDING);
    }

    public void TrySkip()
    {
        Data.StartStandingCooldown();
        Data.ForceTransferByStanding();
        OnSkipped?.Invoke();
    }

    public void TryFallAsleep()
    {
        ChangeState(PlayerState.DEEPSLEEP);
    }

    private void OnDestroy()
    {
        // 순수 C# 데이터 객체이므로 종료 중에도 null이 되지 않는다 (싱글톤 재접근 회피)
        if (Data != null)
            Data.OnLineEnded -= Data.AddStandingCount;

        if (_tiredness != null)
            _tiredness.OnTiredChange -= HandleTiredChange;
    }
}