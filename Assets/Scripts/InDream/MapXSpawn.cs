using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MapXSpawn : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> mapList;

    int randomint;  //타일 번호 랜덤 변수
    private int SpawnedIndex; //스폰된 위치 변수
    float nextSpawnTransform;  
    public float tileLength = 8f;
  

    //맵 길이 제한
    public int maxTileCount;
    private int spawnedCount;


    public void SetIndex(int index)
    {
        SpawnedIndex = index;
    }


    void Start()
    {
        if (SpawnedIndex == 0) //캐릭터가 왼쪽 -> 오른쪽일 때
        {
            nextSpawnTransform = player.transform.position.x + tileLength / 2f - 1f;
        }
        if (SpawnedIndex == 1) //캐릭터가 오른쪽-> 왼쪽일 때
        {
            nextSpawnTransform = player.transform.position.x - tileLength / 2f + 1f;
        }
    }

    void Update()
    { 
        if (SpawnedIndex == 0) //캐릭터가 왼쪽 -> 오른쪽일 때
        {
            if (player.transform.position.x > nextSpawnTransform)
            {
                MapXSpawnToRight();
            }
        }

        if (SpawnedIndex == 1) //캐릭터가 오른쪽-> 왼쪽일 때
        {
            if (player.transform.position.x < nextSpawnTransform)
            {
                MapXSpawnToLeft();
            }
        }
        
    }

    void MapXSpawnToRight() //오른쪽으로 맵 생성
    {
        randomint = Random.Range(0, mapList.Count); //맵패턴 숫자만큼까지 랜덤으로

        Instantiate(mapList[randomint], transform.position, Quaternion.identity);
        nextSpawnTransform = player.transform.position.x + tileLength;
        transform.position = new Vector3(transform.position.x + tileLength, transform.position.y,0);
    }

    void MapXSpawnToLeft() //왼쪽으로 맵 생성
    {
        randomint = Random.Range(0, mapList.Count); 

        Instantiate(mapList[randomint], transform.position, Quaternion.identity);
        nextSpawnTransform = player.transform.position.x - tileLength;
        transform.position = new Vector3(transform.position.x - tileLength, transform.position.y,0);
    }
    
}
