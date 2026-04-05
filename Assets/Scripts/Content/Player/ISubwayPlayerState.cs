using UnityEngine;

public enum PlayerState
{
    NONE,
    SLEEP, // 졸고 있음, 지속적으로 피로도 증가
    STANDING, // 입석
    DEEPSLEEP, // 피로도 100, 꿈 속 진입
}

public interface ISubwayPlayerState
{
    PlayerState State { get; }
    void Enter(SubwayPlayerContext player);
    void Exit(SubwayPlayerContext player);

    void HandleSlap(SubwayPlayerContext player);
    void HandleStanding(SubwayPlayerContext player);
    void HandleFallAsleep(SubwayPlayerContext player);
}
