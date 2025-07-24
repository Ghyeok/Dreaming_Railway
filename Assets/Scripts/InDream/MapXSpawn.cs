using UnityEngine;
using System.Collections.Generic;



public class MapXSpawn : MonoBehaviour
{    public GameObject player;
    public List<GameObject> mapList;


    public float groundY;
    public float tileLength;
    public float startSpawnOffset; // 스폰 시작 설정점

    float nextSpawnDistanceRight; // 오른쪽 스폰 기준점
    float nextSpawnDistanceLeft;  // 왼쪽 스폰 기준점


    public bool canSpawnRight = false;
    public bool canSpawnLeft = false; //기본맵 생성

   
    private bool mapLengthLogged = false; //맵 길이 디버그 위함
    private bool lastRandomMapSpawn = false; 
    private bool endMapSpawn = false;


    private int spawnedCount;  //맵 길이 제한
    public float ExitPointXPosition; //카메라, 안개 제한 위함
    private int SpawnedIndex;



    void Start()
    {

        nextSpawnDistanceRight = tileLength;
        nextSpawnDistanceLeft = -tileLength;
        spawnedCount = 0;

        if (SpawnedIndex == 2)
        {
            canSpawnRight = false;
            canSpawnLeft = false;
        }
    }

    public void SetIndex(int index)
    {
        SpawnedIndex = index; //랜덤 생성된 안개 위치를 받아오기 위함 0이 왼, 1이 오, 2가 아래
    }


    void Update()
    {
        if (SpawnedIndex == 0 || SpawnedIndex == 1)
        {
            // 오른쪽 이동 및 스폰
            if (!canSpawnRight && player.transform.position.x > startSpawnOffset)
            {
                canSpawnRight = true;
                Debug.Log("오른쪽 스폰 시작");
            }

            if (!lastRandomMapSpawn && canSpawnRight && player.transform.position.x + 20f > nextSpawnDistanceRight - tileLength /2f)
            {
                MapXSpawnToRight();
            }

            // 왼쪽 이동 및 스폰
            if (!canSpawnLeft && player.transform.position.x < -startSpawnOffset)
            {
                canSpawnLeft = true;
                Debug.Log("왼쪽 스폰 시작");
            }

            if (!lastRandomMapSpawn && canSpawnLeft && player.transform.position.x - 20f < nextSpawnDistanceLeft + tileLength /2f)
            {
                MapXSpawnToLeft();
            }
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
                if (spawnedCount >= 4 + SubwayPlayerManager.Instance.slapNum)
                {
                    LimitMapSpawning();
                }
            }

            else if (mapLength >= 2)
            {//평균 클리어 타임 40~45초
                if (spawnedCount >= 7 + SubwayPlayerManager.Instance.slapNum)
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

            if (player.transform.position.x >= 0f)
            {
                spawnPos = new Vector3(nextSpawnDistanceRight, groundY, 0f);
            }
            else
            {
                spawnPos = new Vector3(nextSpawnDistanceLeft, groundY, 0f);
            }

            GameObject spawnedLastMap = Instantiate(mapList[10], spawnPos, Quaternion.identity);


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



