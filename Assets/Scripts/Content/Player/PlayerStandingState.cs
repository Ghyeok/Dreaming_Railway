using UnityEngine;

public class PlayerStandingState : ISubwayPlayerState
{
    public PlayerState State => PlayerState.STANDING;

    public void Enter(SubwayPlayerContext player)
    {
        throw new System.NotImplementedException();
    }

    public void Exit(SubwayPlayerContext player)
    {
        throw new System.NotImplementedException();
    }

    public void HandleFallAsleep(SubwayPlayerContext player)
    {
        throw new System.NotImplementedException();
    }

    public void HandleSlap(SubwayPlayerContext player)
    {
        throw new System.NotImplementedException();
    }

    public void HandleStanding(SubwayPlayerContext player)
    {
        throw new System.NotImplementedException();
    }

    public void Update(SubwayPlayerContext player)
    {
        throw new System.NotImplementedException();
    }
}
