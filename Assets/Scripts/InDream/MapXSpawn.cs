using UnityEngine;
using System.Collections.Generic;


public class MapXSpawn : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> mapList;

    int randomint;  //타일 번호 랜덤 변수
    public float groundY;
    float nextSpawnTransformRight; // 오른쪽 스폰 기준점
    float nextSpawnTransformLeft;  // 왼쪽 스폰 기준점
    public float tileLength;
    public float startSpawnOffset; // 스폰 시작 오프셋

    private bool canSpawnRight = false;
    private bool canSpawnLeft = false;

    //맵 길이 제한
    //public int maxTileCount;
    //private int spawnedCount;

    void Start()
    {
        nextSpawnTransformRight = player.transform.position.x + startSpawnOffset;
        nextSpawnTransformLeft = player.transform.position.x - startSpawnOffset;

    }

    void Update()
    {
        // 오른쪽 이동 및 스폰
        if (!canSpawnRight && player.transform.position.x > startSpawnOffset)
        {
            canSpawnRight = true;
            Debug.Log("오른쪽 스폰 시작");
        }

        if (canSpawnRight && player.transform.position.x > nextSpawnTransformRight)
        {
            MapXSpawnToRight();
        }

        // 왼쪽 이동 및 스폰
        if (!canSpawnLeft && player.transform.position.x < -startSpawnOffset)
        {
            canSpawnLeft = true;
            Debug.Log("왼쪽 스폰 시작");
        }

        if (canSpawnLeft && player.transform.position.x < nextSpawnTransformLeft)
        {
            MapXSpawnToLeft();
        }
    }

    void MapXSpawnToRight() // 오른쪽으로 맵 생성
    {
        int randomint = Random.Range(0, mapList.Count);
        Instantiate(mapList[randomint], transform.position, Quaternion.identity);
        transform.position = new Vector3(transform.position.x + tileLength, groundY, 0f);
        nextSpawnTransformRight += tileLength;
    }

    void MapXSpawnToLeft() // 왼쪽으로 맵 생성
    {
        int randomint = Random.Range(0, mapList.Count);
        Instantiate(mapList[randomint], transform.position, Quaternion.identity);
        transform.position = new Vector3(transform.position.x - tileLength, groundY, 0f);
        nextSpawnTransformLeft -= tileLength;
    }

}
