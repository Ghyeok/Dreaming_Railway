using UnityEngine;

public class SubwayManager : Managers<SubwayManager>
{
    public enum PlayerState
    {
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

    public float maxTired;
    public float currentTired;
    public float tiredIncreaseSpeed;

    public bool isSlapCoolTime;
    public float tiredDecreaseBySlap;

    public PlayerState playerState;
    public PlayerBehave playerBehave;

    public bool isTiredHalf;

    public override void Awake()
    {
        playerState = PlayerState.SLEEP;
        playerBehave = PlayerBehave.NONE;
        maxTired = 100f;
        currentTired = 51f;
        isTiredHalf = true;
        tiredIncreaseSpeed = 1f; // 매초 피로도 1씩 증가
        tiredDecreaseBySlap = 3f; // 뺨 때릴시 피로도 3 감소
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckTired();
    }

    void CheckTired()
    {
        currentTired = Mathf.Clamp(currentTired, 0, maxTired);

        if (currentTired > maxTired / 2)
        {
            isTiredHalf = true;
        }
        else
        {
            isTiredHalf = false;
        }
    }
}
