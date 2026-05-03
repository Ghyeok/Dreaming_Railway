using UnityEngine;

public class PlayerSleepState : ISubwayPlayerState
{
    public PlayerState State => PlayerState.SLEEP;

    public void Enter(SubwayPlayerContext player)
    {

    }

    public void Update(SubwayPlayerContext player)
    {
        throw new System.NotImplementedException();
    }

    public void Exit(SubwayPlayerContext player)
    {

    }
}
