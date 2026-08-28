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
    private SubwayData _subway;
    private TimerData _timer;
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
        _timer = GameDataManager.Instance.Timer;

        _subway = GameDataManager.Instance.Subway;
        _subway.ResetPlayerSession(_game.GameMode == GameMode.InfiniteMode);
        Anim = GetComponent<Animator>();

        _subway.OnLineEnded -= _subway.AddStandingCount;
        _subway.OnLineEnded += _subway.AddStandingCount;

        ChangeState(PlayerState.SLEEP);
        _tiredness.OnTiredChange += HandleTiredChange;
    }

    private void Update()
    {
        if (_timer == null || _timer.IsPaused) return;

        _subway.TickSlapCooldown(Time.deltaTime);
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
        if (CurrentState == PlayerState.NONE || _subway.IsSlapCoolTime) return;

        _subway.StartSlapCooldown();
        SoundManager.Instance.SlapSFX();
        Anim.SetTrigger("isSlap");
        _tirednessManager.DecreaseTiredness(_subway.TiredDecreaseBySlap);
        OnSlapSuccessed?.Invoke();
    }

    public void TryStand()
    {
        if (_subway.IsStandingCoolDown) return;
        ChangeState(PlayerState.STANDING);
    }

    public void TrySkip()
    {
        _subway.StartStandingCooldown();
        _subway.ForceTransferByStanding();
        OnSkipped?.Invoke();
    }

    public void TryFallAsleep()
    {
        ChangeState(PlayerState.DEEPSLEEP);
    }

    private void OnDestroy()
    {
        // 순수 C# 데이터 객체이므로 종료 중에도 null이 되지 않는다 (싱글톤 재접근 회피)
        if (_subway != null)
            _subway.OnLineEnded -= _subway.AddStandingCount;

        if (_tiredness != null)
            _tiredness.OnTiredChange -= HandleTiredChange;
    }
}