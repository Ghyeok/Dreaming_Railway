using System;
using UnityEngine;

public enum PlayerState
{
    NONE,
    SLEEP,
    STANDING,
    DEEPSLEEP,
}

public class SubwayPlayerContext : MonoBehaviour, ITickable
{
    private TirednessManager _tirednessManager;
    public SubwayData Rule { get; private set; }
    public Animator Anim { get; private set; }

    public event Action<PlayerState> OnStateChanged;
    public event Action OnSlapSuccessed;

    public PlayerState CurrentState { get; private set; } = PlayerState.NONE;

    public void Init(TirednessManager tirednessManager)
    {
        _tirednessManager = tirednessManager;
        Rule = new SubwayData();
        Rule.Reset(GameManager.Instance.GameMode == GameMode.InfiniteMode);
        Anim = GetComponent<Animator>();

        if (TimerManager.Instance != null)
            TimerManager.Instance.Register(this);

        SubwayFlowManager.Instance.OnLineEnded += Rule.AddStandingCount;

        ChangeState(PlayerState.SLEEP);
        TirednessManager.OnTiredChange += HandleTiredChange;
    }

    public void Tick(float deltaTime)
    {
        Rule.TickSlapCooldown(deltaTime);
    }

    public void ForceMaxTiredness() => _tirednessManager.SetTirednessForced(100f);

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
            Anim.SetBool("isSleeping", TirednessManager.IsTiredHalf);
    }

    public void TryTransfer()
    {
        Anim.SetTrigger("isTransfer");
    }

    public void TrySlap()
    {
        if (CurrentState == PlayerState.NONE || Rule.IsSlapCoolTime) return;

        Rule.StartSlapCooldown();
        SoundManager.Instance.SlapSFX();
        Anim.SetTrigger("isSlap");
        _tirednessManager.DecreaseTiredness(Rule.TiredDecreaseBySlap);
        OnSlapSuccessed?.Invoke();
    }

    public void TryStand()
    {
        if (Rule.IsStandingCoolDown) return;

        Rule.StartStandingCooldown();
        ChangeState(PlayerState.STANDING);
    }

    public void TryFallAsleep()
    {
        ChangeState(PlayerState.DEEPSLEEP);
    }

    private void OnDestroy()
    {
        if (TimerManager.Instance != null)
            TimerManager.Instance.Unregister(this);

        if (SubwayFlowManager.Instance != null)
            SubwayFlowManager.Instance.OnLineEnded -= Rule.AddStandingCount;

        TirednessManager.OnTiredChange -= HandleTiredChange;
    }
}