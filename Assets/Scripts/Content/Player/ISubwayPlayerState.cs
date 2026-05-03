using UnityEngine;

public interface ISubwayPlayerState
{
    PlayerState State { get; }
    void Enter(SubwayPlayerContext player);
    void Update(SubwayPlayerContext player);
    void Exit(SubwayPlayerContext player);
}
