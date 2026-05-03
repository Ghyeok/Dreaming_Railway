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

    [SerializeField] private ISubwayPlayerState _currentState;
    public PlayerState CurrentState
    {
        get { return _currentState != null ? _currentState.State : PlayerState.NONE; }
    }

    public void Init(TirednessManager tirednessManager)
    {
        _tirednessManager = tirednessManager;
        Rule = new SubwayData();
        Rule.Reset(GameManager.Instance.GameMode == GameMode.InfiniteMode);
        Anim = GetComponent<Animator>();

        if (TimerManager.Instance != null)
            TimerManager.Instance.Register(this);

        SubwayFlowManager.Instance.OnLineEnded += Rule.AddStandingCount;

        ChangeState(new PlayerSleepState());
        TirednessManager.OnTiredChange += HandleTiredChange;
    }

    public void Tick(float deltaTime)
    {
        Rule.TickSlapCooldown(deltaTime);
    }

    public void ForceMaxTiredness() => _tirednessManager.SetTirednessForced(100f);

    public void ChangeState(ISubwayPlayerState newState)
    {
        if (_currentState != null)
            _currentState.Exit(this);

        _currentState = newState;

        if (_currentState != null)
        {
            _currentState.Enter(this);
            OnStateChanged?.Invoke(CurrentState);
        }
    }

    public void HandleTiredChange(float currentTired)
    {
        if (CurrentState == PlayerState.SLEEP)
        {
            Anim.SetBool("isSleeping", TirednessManager.IsTiredHalf);
        }
    }

    public void TryTransfer()
    {
        if (_currentState != null)
            Anim.SetTrigger("isTransfer");
    }

    public void TrySlap()
    {
        if (_currentState == null || Rule.IsSlapCoolTime) return;

        Rule.StartSlapCooldown();
        SoundManager.Instance.SlapSFX();
        Anim.SetTrigger("isSlap");
        _tirednessManager.DecreaseTiredness(Rule.TiredDecreaseBySlap);
        OnSlapSuccessed?.Invoke();
    }

    public void TryStand()
    {
        if (_currentState == null || Rule.IsStandingCoolDown) return;

        Rule.StartStandingCooldown();
        ChangeState(new PlayerStandingState());
    }

    public void TryFallAsleep()
    {
        if (_currentState != null)
            ChangeState(new PlayerDeepSleepState());
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