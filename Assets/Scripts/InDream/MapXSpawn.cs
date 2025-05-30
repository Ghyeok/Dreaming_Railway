using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class MapXSpawn : SingletonManagers<MapXSpawn>
{
    public SubwayGameManager subwayGameManager;
    public GameObject player;
    public List<GameObject> mapList;


    public float groundY;
    public float tileLength;
    public float startSpawnOffset; // 스폰 시작 설정점

    float nextSpawnDistanceRight; // 오른쪽 스폰 기준점
    float nextSpawnDistanceLeft;  // 왼쪽 스폰 기준점


    private bool canSpawnRight = false;
    private bool canSpawnLeft = false; //기본맵 생성

   
    private bool mapLengthLogged = false; //맵 길이 디버그 위함
    private bool lastRandomMapSpawn = false; 
    private bool endMapSpawn = false;


    private int spawnedCount;  //맵 길이 제한
    public float ExitPointXPosition; //카메라 제한 위함


    void Start()
    {

        nextSpawnDistanceRight = tileLength;
        nextSpawnDistanceLeft = -tileLength;
        spawnedCount = 0;

    }


    void Update()
    {

        // 오른쪽 이동 및 스폰
        if (!canSpawnRight && player.transform.position.x > startSpawnOffset)
        {
            canSpawnRight = true;
            Debug.Log("오른쪽 스폰 시작");
        }

        if (!lastRandomMapSpawn && canSpawnRight && player.transform.position.x + 15f > nextSpawnDistanceRight - tileLength /2f)
        {
            MapXSpawnToRight();
        }

        // 왼쪽 이동 및 스폰
        if (!canSpawnLeft && player.transform.position.x < -startSpawnOffset)
        {
            canSpawnLeft = true;
            Debug.Log("왼쪽 스폰 시작");
        }

        if (!lastRandomMapSpawn && canSpawnLeft && player.transform.position.x - 15f < nextSpawnDistanceLeft + tileLength /2f)
        {
            MapXSpawnToLeft();
        }

        // 맵 길이
        int mapLength = SubwayGameManager.Instance.SetDreamMapLengthByAwakenTime();
        if (!mapLengthLogged) // 디버그 로그는 한 번만 출력
        {
            Debug.Log("맵 길이: " + mapLength);
            mapLengthLogged = true;
        }


        if (!endMapSpawn)
        {
            if (mapLength == 1)
            {//평균 클리어 타임 22~25초
                if (spawnedCount >= 10 + 2 * SubwayPlayerManager.Instance.slapNum)
                {
                    LimitMapSpawning();
                }
            }

            else if (mapLength == 2)
            {//평균 클리어 타임 40~45초
                if (spawnedCount >= 18 +2 * SubwayPlayerManager.Instance.slapNum)
                {
                    LimitMapSpawning();
                }
            }
        }
    }
    

    void MapXSpawnToRight() // 오른쪽으로 맵 생성
    {
        int randomint = Random.Range(0, mapList.Count - 1);
        Vector3 spawnPos = new Vector3(nextSpawnDistanceRight, groundY, 0f);
        Instantiate(mapList[randomint], spawnPos, Quaternion.identity);
        nextSpawnDistanceRight += tileLength;

        spawnedCount++; // 맵 생성 수 증가


    }

    void MapXSpawnToLeft() // 왼쪽으로 맵 생성
    {
        int randomint = Random.Range(0, mapList.Count - 1);
        Vector3 spawnPos = new Vector3(nextSpawnDistanceLeft, groundY, 0f);
        Instantiate(mapList[randomint], spawnPos, Quaternion.identity);
        nextSpawnDistanceLeft -= tileLength;

        spawnedCount++; // 맵 생성 수 증가

    }

    //^^^^ 기본 맵 생성 로직




    void LimitMapSpawning()
    {
        endMapSpawn = true;    //탈출구 생성
        lastRandomMapSpawn = true;  //그만 랜덤 생성, 마지막 랜덤맵 스폰됨
        SpawnExit();
    }
        


    void SpawnExit() //탈출
    {
        if (endMapSpawn)
        {
            Vector3 spawnPos = Vector3.zero;

            if (canSpawnRight)
            {
                spawnPos = new Vector3(nextSpawnDistanceRight, groundY, 0f);
            }
            else if (canSpawnLeft)
            {
                spawnPos = new Vector3(nextSpawnDistanceLeft, groundY, 0f);
            }

            GameObject spawnedLastMap = Instantiate(mapList[7], spawnPos, Quaternion.identity);


            // 프리팹 안의 "ExitDoor"를 찾기
            Transform exitDoor = spawnedLastMap.transform.Find("ExitDoor");

            if (exitDoor != null)
            {
                ExitPointXPosition = exitDoor.position.x;
            }
        }
    }
}



//^^^^ 맵길이 조절 로직



