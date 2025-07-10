using UnityEngine;

public class FogSpawn : SingletonManagers<FogSpawn> 
{
    public GameObject Fog;
    public Vector3[] spawnPositions = new Vector3[3];
    public int randomIndex;


    void Start () 
    {
        SpawnRandomPosition();
    }

    void SpawnRandomPosition()
    {   //안개 생성 위치 랜덤 지정
        randomIndex = Random.Range(0, spawnPositions.Length);

        if (randomIndex == 0) //좌
            transform.rotation = Quaternion.identity;

        else if (randomIndex == 1) //우
            transform.rotation = Quaternion.Euler(0, 0, 180);

        else if (randomIndex == 2) //하
            transform.rotation = Quaternion.Euler(0, 0, 90);

        //안개 생성
        GameObject FogClone = Instantiate(Fog, spawnPositions[randomIndex], transform.rotation);
    }
}

