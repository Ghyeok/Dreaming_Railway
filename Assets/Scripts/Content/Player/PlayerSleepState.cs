using UnityEngine;

public class PlayerSleepState : ISubwayPlayerState
{
    public PlayerState State => PlayerState.SLEEP;

    public void Enter(SubwayPlayerContext player)
    {

    }

    public void Exit(SubwayPlayerContext player) { }

    public void HandleFallAsleep(SubwayPlayerContext player)
    {
       
    }

    public void HandleSlap(SubwayPlayerContext player)
    {

    }

    public void HandleStanding(SubwayPlayerContext player)
    {
        
    }

    public void Update(SubwayPlayerContext player)
    {
        
    }
}
