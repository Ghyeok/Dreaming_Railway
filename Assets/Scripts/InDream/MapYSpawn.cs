using UnityEngine;
using System.Collections.Generic;

public class MapYSpawn : MonoBehaviour
{
    //public SubwayGameManager subwayGameManager;
    public GameObject player;
    public List<GameObject> mapList;

    private float nextSpawnY = 0f;
    private float cameraHeight;
   
    private bool canSpawnToUp = true;

    private int spawnedCount; //맵길이 제한
    private bool endMapSpawn = false;
    public float ExitPointYPosition;
    private int SpawnedIndex;


    void Start()
    {
        MapXSpawn mapXSpawn = GameObject.Find("MapXSpawner").GetComponent<MapXSpawn>();

        if (SpawnedIndex == 0 || SpawnedIndex == 1)
        {
            canSpawnToUp = false;
        }

        if (SpawnedIndex == 2)
        {
            cameraHeight = Camera.main.orthographicSize * 1.5f;
            
            //시작 시 3개 미리 생성
            for (int i = 0; i < 3; i++)
            {
                MapYSpawnToUp();
            }
        }
    }

    public void SetIndex(int index)
    {
        SpawnedIndex = index; //랜덤 생성된 안개 위치를 받아오기 위함 0이 왼, 1이 오, 2가 아래
    }

    void Update()
    {
        //카메라보다 위에 미리 맵이 없으면 생성
        if (player.transform.position.y + cameraHeight > nextSpawnY)
        {
            MapYSpawnToUp();
        }


        // 맵 길이
        int mapLength = SubwayGameManager.Instance.SetDreamMapLengthByAwakenTime();

        if (!endMapSpawn)
        {
            if (mapLength == 1)
            {//평균 클리어 타임 22~25초
                if (spawnedCount >= 15 + 2 * SubwayPlayerManager.Instance.slapNum)
                {
                    LimitMapSpawning();
                }
            }

            else if (mapLength >= 2)
            {//평균 클리어 타임 40~45초
                if (spawnedCount >= 24 + 2 * SubwayPlayerManager.Instance.slapNum)
                {
                    LimitMapSpawning();
                }
            }
        }
    }

    void MapYSpawnToUp()
    {
        if (canSpawnToUp)
        {
            int randomint = Random.Range(0, mapList.Count - 1);
            float randomX = Random.Range(-5f, 5f);
            Instantiate(mapList[randomint], new Vector3(randomX, nextSpawnY, 0), Quaternion.identity);

            //다음 생성 위치 설정
            nextSpawnY += 5f;

            // 맵 생성 수 증가
            spawnedCount++;
        }

    }
    
    //^^^기본 맵 로직
    void LimitMapSpawning()
    {
        endMapSpawn = true;    //탈출구 생성
        canSpawnToUp = false;
        SpawnExit();
    }

    void SpawnExit() //탈출
    {
        if (endMapSpawn)
        {
            GameObject spawnedLastMap = Instantiate(mapList[10], new Vector3(0, nextSpawnY, 0), Quaternion.identity);


            // 프리팹 안의 "ExitDoor"를 찾기
            Transform exitDoor = spawnedLastMap.transform.Find("ExitDoor");

            if (exitDoor != null)
            {
                ExitPointYPosition = exitDoor.position.y;
            }
        }
    }
}


