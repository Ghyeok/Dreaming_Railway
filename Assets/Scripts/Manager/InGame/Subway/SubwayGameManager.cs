using UnityEngine;

/* 플레이어가 지하철에 있을 때의 전체적인 흐름을 관리하는 매니저이다.
 * 게임 모드, 뺨 치기 쿨타임, 뺨 치기 피로도 감소량, 현재 Day 몇인지 등등을 다룬다.
 */
public class SubwayGameManager : SingletonManagers<SubwayGameManager>
{
    public bool isSlapCoolTime;
    public float tiredDecreaseBySlap;
    public int dayCount;

    public bool isTraveling; // 운행 중
    public bool isStopping; // 정차 중

    [SerializeField]
    public Timer timer;

    public override void Awake()
    {
        base.Awake();
        isTraveling = false;
        isStopping = false;
        isSlapCoolTime = false;
        tiredDecreaseBySlap = 3f;
        dayCount = 1;
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
