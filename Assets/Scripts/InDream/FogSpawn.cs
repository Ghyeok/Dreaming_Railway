using UnityEngine;

public class FogSpawn : MonoBehaviour 
{
    public GameObject Fog;
    public Vector3[] spawnPositions = new Vector3[3];
    private int randomIndex;


    void Start () 
    {
        SpawnRandomPosition();
    }

    void SpawnRandomPosition()
    {   //안개 생성 위치 랜덤 지정
        randomIndex = UnityEngine.Random.Range(0, spawnPositions.Length);

        if (randomIndex == 0) //좌
            transform.rotation = Quaternion.identity;

        else if (randomIndex == 1) //우
            transform.rotation = Quaternion.Euler(0, 0, 180);

        else if (randomIndex == 2) //하
            transform.rotation = Quaternion.Euler(0, 0, 90);

        //안개 생성
        GameObject FogClone = Instantiate(Fog, spawnPositions[randomIndex], transform.rotation);


        //랜덤인덱스값 전달
        FogMovement movement = FogClone.GetComponent<FogMovement>();
        if (movement != null)
        {
            movement.SetIndex(randomIndex);
        }

        Camera_move cameraMove = Camera.main.GetComponent<Camera_move>();
        if (cameraMove != null)
        {
            cameraMove.SetIndex(randomIndex);
        }

        MapYSpawn MapYMove = GameObject.Find("MapYSpawner").GetComponent<MapYSpawn>();
        if (MapYMove != null)
        {
            MapYMove.SetIndex(randomIndex);
        }

        MapXSpawn MapXMove = GameObject.Find("MapXSpawner").GetComponent<MapXSpawn>();
        if (MapXMove != null)
        {
            MapXMove.SetIndex(randomIndex);
        }
    }
}

