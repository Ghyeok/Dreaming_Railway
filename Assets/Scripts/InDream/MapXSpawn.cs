using UnityEngine;
using System.Collections.Generic;


public class MapXSpawn : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> mapList;

    public float groundY;
    float nextSpawnTransformRight; // 오른쪽 스폰 기준점
    float nextSpawnTransformLeft;  // 왼쪽 스폰 기준점
    public float tileLength;
    public float startSpawnOffset; // 스폰 시작 설정점

    private bool canSpawnRight = false;
    private bool canSpawnLeft = false; //기본맵 생성


    //맵 길이 제한
    private int spawnedCount;
  

    void Start()
    {
        nextSpawnTransformRight = tileLength;  
        nextSpawnTransformLeft = -tileLength; 

    }

    void Update()
    {
        // 오른쪽 이동 및 스폰
        if (!canSpawnRight && player.transform.position.x > startSpawnOffset)
        {
            canSpawnRight = true;
            Debug.Log("오른쪽 스폰 시작");
        }

        if (canSpawnRight && player.transform.position.x + tileLength/1.2f > nextSpawnTransformRight)
        {
            MapXSpawnToRight();
        }

        // 왼쪽 이동 및 스폰
        if (!canSpawnLeft && player.transform.position.x < -startSpawnOffset)
        {
            canSpawnLeft = true;
            Debug.Log("왼쪽 스폰 시작");
        }

        if (canSpawnLeft && player.transform.position.x - tileLength/1.2f < nextSpawnTransformLeft)
        {
            MapXSpawnToLeft();
        }
    }

    void MapXSpawnToRight() // 오른쪽으로 맵 생성
    {
        int randomint = Random.Range(0, mapList.Count);
        Vector3 spawnPos = new Vector3(nextSpawnTransformRight, groundY, 0f);
        Instantiate(mapList[randomint], spawnPos, Quaternion.identity);
        nextSpawnTransformRight += tileLength;

        spawnedCount++; // 맵 생성 수 증가
    }

    void MapXSpawnToLeft() // 왼쪽으로 맵 생성
    {
        int randomint = Random.Range(0, mapList.Count);
        Vector3 spawnPos = new Vector3(nextSpawnTransformLeft, groundY, 0f);
        Instantiate(mapList[randomint], spawnPos, Quaternion.identity);
        nextSpawnTransformLeft -= tileLength;

        spawnedCount++; // 맵 생성 수 증가
    }

//^^^^ 기본 맵 생성 로직


}


 /* 

if ( Manager(파일명).Instance.(깨어있던 시간) < 50 )
{//평균 클리어 타임 22~25초
    if ( spawnedCount == 10 +  2*Manager.Instance.(뺨 때린 횟수) )
    {   
        canSpawnRight = false;
        canSpawnleft = false;
    }
  
else if ( Manager.Instance.(깨어잇던 시간) >= 50s )
//평균 클리어 타임 40~45초
    if ( spawnedCount == 18 + 2*Manager.Instance.(뺨 때린 횟수) )
    {   canSpawnRight = false;
        canSpawnleft = false;
    }


//^^^^ 맵길이 조절절 로직


*/
