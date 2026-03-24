using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerBehave
{
    NONE,
    SLAP, // 뺨 때리기
    FALLASLEEP, // 즉시 잠들기
    TRANSFER, // 환승하기
    GETOFF, // 목적지에 내리기
}

public class SubwayPlayerContext : MonoBehaviour
{
    public SubwaySceneRefs Refs {  get; private set; }
    public Animator Anim {  get; private set; }

    public PlayerState CurrentState
    {
        get
        {
            return _currentState != null?_currentState.State : PlayerState.NONE;
        }
    }

    public PlayerBehave CurrentBehave {  get; private set; }
    public int SlapNum {  get; private set; }

    [SerializeField] private ISubwayPlayerState _currentState;

    public void Init(SubwaySceneRefs refs)
    {
        Refs = refs;
        Anim = GetComponent<Animator>();
        SlapNum = 0;

        Refs.tirednessManager.OnTiredChange += HandleTiredChange;

        ChangeState(new PlayerSleepState());
    }

    public void ChangeState(ISubwayPlayerState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit(this);
        }

        _currentState = newState;

        if (_currentState != null)
        {
            _currentState.Enter(this);
        }
    }

    public void HandleTiredChange(float currentTired)
    {
        if (CurrentState == PlayerState.SLEEP)
        {
            bool isSleeping = Refs.tirednessManager.IsTiredHalf;
            Anim.SetBool("isSleeping", isSleeping);
        }
    }

    public void TrySlap()
    {
        if (_currentState != null)
        {
            _currentState.HandleSlap(this);
        }
    }

    public void TryStand()
    {
        if (_currentState != null)
        {
            _currentState.HandleStanding(this);
        }
    }

    public void TryFallAsleep()
    {
        if (_currentState != null)
        {
            _currentState.HandleFallAsleep(this);
        }
    }
}
