using UnityEngine;

public class PlayerDeepSleepState : ISubwayPlayerState
{
    public PlayerState State => PlayerState.DEEPSLEEP;

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
