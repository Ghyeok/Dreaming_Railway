using UnityEngine;

public class SubwayManager : Managers<SubwayManager>
{
    public enum PlayerState
    {
        AWAKE,
        SLEEP,
        DEEPSLEEP,
    }

    public float maxTired;
    public float currentTired;
    public float tiredIncreaseSpeed;

    public PlayerState playerState;

    public override void Awake()
    {
        playerState = PlayerState.SLEEP;
        maxTired = 100f;
        tiredIncreaseSpeed = 2f;
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
