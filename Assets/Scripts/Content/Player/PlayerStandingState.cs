using UnityEngine;

public class PlayerStandingState : ISubwayPlayerState
{
    public PlayerState State => PlayerState.STANDING;

    public void Enter(SubwayPlayerContext player)
    {
        SoundManager.Instance.StandingSFX();
        player.Anim.SetTrigger("isStanding");
        player.ForceMaxTiredness();
    }
    public void Update(SubwayPlayerContext player)
    {
        throw new System.NotImplementedException();
    }

    public void Exit(SubwayPlayerContext player)
    {

    }
}