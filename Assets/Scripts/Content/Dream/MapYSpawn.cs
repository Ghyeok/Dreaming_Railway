using UnityEngine;
using System.Collections.Generic;

public class MapYSpawn : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> mapList;

    private float nextSpawnY = 0f;
    private float cameraHeight;
   
    private bool canSpawnToUp = true;

    private int spawnedCount; //留듦만???쒗븳
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
            
            //?쒖옉 ??3媛?誘몃━ ?앹꽦
            for (int i = 0; i < 3; i++)
            {
                MapYSpawnToUp();
            }
        }
    }

    public void SetIndex(int index)
    {
        SpawnedIndex = index; //?쒕뜡 ?앹꽦???덇컻 ?꾩튂瑜?諛쏆븘?ㅺ린 ?꾪븿 0???? 1???? 2媛 ?꾨옒
    }

    void Update()
    {
        //移대찓?쇰낫???꾩뿉 誘몃━ 留듭씠 ?놁쑝硫??앹꽦
        if (player.transform.position.y + cameraHeight > nextSpawnY)
        {
            MapYSpawnToUp();
        }


        // 留?湲몄씠
        int mapLength = 0;

        if (!endMapSpawn)
        {
            if (mapLength == 1)
            {//?됯퇏 ?대━?????22~25珥?
                if (spawnedCount >= 15 + DreamManager.Instance.SlapNum)
                {
                    LimitMapSpawning();
                }
            }

            else if (mapLength >= 2)
            {//?됯퇏 ?대━?????40~45珥?
                if (spawnedCount >= 22 + DreamManager.Instance.SlapNum)
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
            int randomint = UnityEngine.Random.Range(0, mapList.Count - 1);
            float randomX = UnityEngine.Random.Range(-5f, 5f);
            Instantiate(mapList[randomint], new Vector3(randomX, nextSpawnY, 0), Quaternion.identity);

            //?ㅼ쓬 ?앹꽦 ?꾩튂 ?ㅼ젙
            nextSpawnY += 5f;

            // 留??앹꽦 ??利앷?
            spawnedCount++;
        }

    }
    
    //^^^湲곕낯 留?濡쒖쭅
    void LimitMapSpawning()
    {
        endMapSpawn = true;    //?덉텧援??앹꽦
        canSpawnToUp = false;
        SpawnExit();
    }

    void SpawnExit() //?덉텧
    {
        if (endMapSpawn)
        {
            GameObject spawnedLastMap = Instantiate(mapList[10], new Vector3(0, nextSpawnY, 0), Quaternion.identity);


            // ?꾨━???덉쓽 "ExitDoor"瑜?李얘린
            Transform exitDoor = spawnedLastMap.transform.Find("ExitDoor");

            if (exitDoor != null)
            {
                ExitPointYPosition = exitDoor.position.y;
            }
        }
    }
}


