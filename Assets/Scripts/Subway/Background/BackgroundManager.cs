using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject undergroundLayer;

    [SerializeField] private Sprite underground;
    [SerializeField] private Sprite station;
    [SerializeField] private Sprite connectR;
    [SerializeField] private Sprite connectL;
    [SerializeField] private Sprite hangang;
    [SerializeField] private Sprite grass;

    public enum BackgroundType
    {
        Underground,
        Hangang,
        Grass,
        Station,
        ConnectR,
        ConnectL,
    }

    public Queue<BackgroundType> backgroundQueue;
    public bool needConnector;
    public BackgroundType currentType;

    void Awake()
    {
        if (backgroundQueue == null)
            backgroundQueue = new Queue<BackgroundType>();

        if (backgroundQueue.Count == 0)
        {
            backgroundQueue.Enqueue(BackgroundType.Underground);
            currentType = BackgroundType.Underground;
        }
    }

    /*
    * 이동 속도
    * 1. 지하 : 5000
    * 2. 한강 : 50 ~ 100
    * 3. 풀숲 : 300 ~ 500
    * 4. 정차역 : 5000 ~ 0 ~ 5000 
    */
    public float SetScrollSpeed(BackgroundType type)
    {
        if (type == BackgroundType.Underground) return 3000f;
        if (type == BackgroundType.Station) return 3000f;
        if (type == BackgroundType.ConnectL) return 3000f;
        if (type == BackgroundType.ConnectR) return 3000f;
        if (type == BackgroundType.Hangang) return 200f;
        if (type == BackgroundType.Grass) return 200f;

        return 0;
    }

    /// <summary>
    /// 현재 출력되고 있는 배경이 사라지는 순간 호출
    /// </summary>
    public void DecideNextBackground()
    {
        int remain = StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].transferIdx - StationManager.Instance.currentStationIdx;
        int rand = Random.Range(1, 101); // 1 ~ 100의 랜덤한 숫자

        // 1. 배경이 바뀌는 순간에 남은 역이 1개이고 정차 구간이라면
        if (remain <= 1 && SubwayGameManager.Instance.isStopping)
        {
            backgroundQueue.Enqueue(BackgroundManager.BackgroundType.Station);
        }
        else // 2. 지하 배경 9, 한강 배경 0.5, 풀 배경 0.5 가중치로 등장
        {
            if (rand <= 5)
                backgroundQueue.Enqueue(BackgroundManager.BackgroundType.Hangang);
            else if (rand > 5 && rand <= 10)
                backgroundQueue.Enqueue(BackgroundManager.BackgroundType.Grass);
            else
                backgroundQueue.Enqueue(BackgroundManager.BackgroundType.Underground);
        }

        SettingNextBackground();
    }

    /// <summary>
    /// 다음 배경이 한강 또는 풀 이면 커넥터 플래그를 세우고, 다음 배경 스크롤 속도를 설정
    /// </summary>
    private void SettingNextBackground()
    {
        if (backgroundQueue.Count > 0)
        {
            BackgroundType nextType = backgroundQueue.Peek();
            currentType = backgroundQueue.Peek();

            if (nextType == BackgroundType.Hangang || nextType == BackgroundType.Grass)
            {
                needConnector = true;
            }
            else
            {
                needConnector = false;
            }
        }
    }

    public Sprite ReturnBackgroundImage()
    {
        BackgroundType type = backgroundQueue.Dequeue();

        if (type == BackgroundType.Underground) return underground;
        if (type == BackgroundType.Station) return station;
        if (type == BackgroundType.ConnectR) return connectR;
        if (type == BackgroundType.ConnectL) return connectL;
        if (type == BackgroundType.Hangang) return hangang;
        if (type == BackgroundType.Grass) return grass;
        else return null;
    }


    private void OnEnable()
    {
        BackgroundScroller.OnBackgroundChange += DecideNextBackground;
    }

    private void OnDisable()
    {
        BackgroundScroller.OnBackgroundChange -= DecideNextBackground;
    }
}   
