using UnityEngine;


// 지하철 내 플레이어의 행동, 상태를 관리하는 매니저이다.
public class SubwayPlayerManager : SingletonManagers<SubwayPlayerManager>
{
    public enum PlayerState
    {
        NONE,
        STANDING, // 입석
        SLEEP, // 졸고 있음
        DEEPSLEEP, // 피로도 100
        GAMEOVER,
    }

    public enum PlayerBehave
    {
        NONE,
        SLAP, // 뺨 때리기
        FALLASLEEP, // 즉시 잠들기
        TRANSFER, // 환승하기
        GETOFF, // 목적지에 내리기
    }

    public PlayerState playerState;
    public PlayerBehave playerBehave;

    public int slapNum = 0;

    public override void Awake()
    {
        base.Awake();
        playerState = PlayerState.SLEEP;
        playerBehave = PlayerBehave.NONE;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
