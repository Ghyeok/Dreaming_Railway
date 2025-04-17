using UnityEngine;
using System.Collections.Generic;

public class MapYSpawn : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> mapList;

    int randomint;  //타일 번호 랜덤 변수
    float nextSpawnTransform;
     float distanceCenter;

    void Start()
    {
        distanceCenter = transform.position.y;
        StartMapYSpawn();
    }

    void Update()
    {
        if (player.transform.position.y > nextSpawnTransform)
        {
            StartMapYSpawn();
        }
    }

    void StartMapYSpawn()
    {
        randomint = Random.Range(0, mapList.Count); //맵패턴 숫자만큼까지 랜덤으로

        Instantiate(mapList[randomint], transform.position, Quaternion.identity);
        nextSpawnTransform = player.transform.position.y + 10f;
        transform.position = new Vector3(transform.position.x, transform.position.y + 10f,0);
    }
}
