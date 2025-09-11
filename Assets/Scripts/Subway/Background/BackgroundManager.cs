using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
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

    [SerializeField] public Queue<BackgroundType> backgroundQueue;

    public bool isHangangShown;
    private int grassLoop = 4;
    public bool isGrassShown;
    public bool isTransferRecently;

    public float lastSpeedBeforeStation;

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

        isTransferRecently = false;
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
        if (type == BackgroundType.Underground) return 6000f;
        if (type == BackgroundType.Station) return 6000f;
        if (type == BackgroundType.ConnectL) return 6000f;
        if (type == BackgroundType.ConnectR) return 6000f;
        if (type == BackgroundType.Hangang) return 75f;
        if (type == BackgroundType.Grass) return 3000f;

        return 0;
    }

    public void DecideNextBackground()
    {
        int remain = StationManager.Instance.subwayLines[StationManager.Instance.currentLineIdx].transferIdx - StationManager.Instance.currentStationIdx;
        int rand = Random.Range(1, 101); // 1 ~ 100의 랜덤한 숫자

        bool isOutside = (currentType == BackgroundType.ConnectR ||
                          currentType == BackgroundType.Hangang ||
                          currentType == BackgroundType.Grass ||
                          currentType == BackgroundType.ConnectL);

        Debug.Log($"현재 큐 길이 : {backgroundQueue.Count}");

        if (isOutside) return;

        // 1. 배경이 바뀌는 순간에 남은 역이 1개이고 정차 구간이라면
        if (remain <= 0 && SubwayGameManager.Instance.isStopping)
        {
            lastSpeedBeforeStation = SetScrollSpeed(currentType);
            backgroundQueue.Enqueue(BackgroundType.Station);
        }
        else // 2. 지하 배경 9, 한강 배경 0.5, 풀 배경 0.5 가중치로 등장, 환승한 직후 몇초는 지하 배경만 나오게
        {
            if (rand <= 10 && !isTransferRecently)
            {
                if (!isHangangShown && rand <= 5)
                {
                    isHangangShown = true;
                    backgroundQueue.Enqueue(BackgroundType.ConnectR);
                    backgroundQueue.Enqueue(BackgroundType.Hangang);
                    backgroundQueue.Enqueue(BackgroundType.ConnectL);
                    backgroundQueue.Enqueue(BackgroundType.Underground);
                }
                else if (!isGrassShown && rand > 5 && rand <= 10)
                {
                    backgroundQueue.Enqueue(BackgroundType.ConnectR);
                    for (int i = 0; i < grassLoop; i++)
                        backgroundQueue.Enqueue(BackgroundType.Grass);
                    backgroundQueue.Enqueue(BackgroundType.ConnectL);
                    backgroundQueue.Enqueue(BackgroundType.Underground);
                }
                else // 한강, 풀 배경이 모두 나온적 있다면 지하 배경을 채워줌
                {
                    backgroundQueue.Enqueue(BackgroundType.Underground);
                }
            }
            else
            {
                backgroundQueue.Enqueue(BackgroundType.Underground);
            }
        }
    }

    public Sprite ReturnBackgroundImage()
    {
        BackgroundType type = backgroundQueue.Dequeue();
        currentType = type;

        if (type == BackgroundType.Underground) return underground;
        if (type == BackgroundType.Station) return station;
        if (type == BackgroundType.ConnectR) return connectR;
        if (type == BackgroundType.ConnectL) return connectL;
        if (type == BackgroundType.Hangang) return hangang;
        if (type == BackgroundType.Grass) return grass;
        else return null;
    }

    private void ResetOutsideBackground()
    {
        isGrassShown = false;
        isHangangShown = false;
    }

    private void OnEnable()
    {
        BackgroundScroller.OnBackgroundChange += DecideNextBackground;
        TransferManager.OnTransferSuccess += ResetOutsideBackground;
    }

    private void OnDisable()
    {
        BackgroundScroller.OnBackgroundChange -= DecideNextBackground;
        TransferManager.OnTransferSuccess -= ResetOutsideBackground;
    }
}   
