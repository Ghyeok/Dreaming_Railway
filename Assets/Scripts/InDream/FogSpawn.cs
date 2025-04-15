using UnityEngine;

public class FogSpawn : MonoBehaviour 
{
    public GameObject Fog;
    public Vector3[] spawnPositions = new Vector3[3];


    void Start () 
    {
        SpawnRandomPosition();
    }

    void SpawnRandomPosition()
    {   //안개 생성 위치 랜덤 지정
        int randomIndex = Random.Range(0, spawnPositions.Length);

        Quaternion rotation;
        if (randomIndex == 0) //왼쪽
            transform.rotation = Quaternion.Euler(0,0,-90);

        else if (randomIndex == 1) // 오른쪽
            transform.rotation = Quaternion.Euler(0, 0, 90);

        else if(randomIndex == 2) // 중앙
            transform.rotation = Quaternion.identity;

        //안개 생성
        Instantiate(Fog, spawnPositions[randomIndex], transform.rotation);
    }


}