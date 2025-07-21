using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BackgroundScroller;

public class BackgroundManager : MonoBehaviour
{
    public BackgroundScroller undergroundLayer;
    public BackgroundScroller hangangLayer;
    public BackgroundScroller grassLayer;
    public BackgroundScroller stationLayer;

    public RectTransform canvasRect;

    public Queue<BackgroundType> backgroundQueue = new Queue<BackgroundType>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasRect = gameObject.transform.parent.GetComponent<RectTransform>();
        SetScrollSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetScrollSpeed()
    { 
        StationManager station = StationManager.Instance;
        float canvasWidth = canvasRect.rect.width;

        // 지하 스크롤 속도, 한강의 스크롤 속도 * 현재 노선의 환승역까지의 개수 * 역 하나 당 지하 배경 개수 
        undergroundLayer.scrollSpeed = canvasWidth / station.GetCurrentLineTotalTime() * station.subwayLines[station.currentLineIdx].transferIdx * 30f;

        // 한강 스크롤 속도
        hangangLayer.scrollSpeed = canvasWidth / station.GetCurrentLineTotalTime() * 4f;

        // 정차역 스크롤 속도
        stationLayer.scrollSpeed = undergroundLayer.scrollSpeed;
    }

    public void EnqueueBackgroundType()
    {

    }
}
