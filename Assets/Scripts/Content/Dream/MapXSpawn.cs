using UnityEngine;
using System.Collections.Generic;



public class MapXSpawn : MonoBehaviour
{    public GameObject player;
    public List<GameObject> mapList;


    public float groundY;
    public float tileLength;
    public float startSpawnOffset; // ?ㅽ룿 ?쒖옉 ?ㅼ젙??

    float nextSpawnDistanceRight; // ?ㅻⅨ履??ㅽ룿 湲곗???
    float nextSpawnDistanceLeft;  // ?쇱そ ?ㅽ룿 湲곗???


    public bool canSpawnRight = false;
    public bool canSpawnLeft = false; //湲곕낯留??앹꽦

   
    private bool mapLengthLogged = false; //留?湲몄씠 ?붾쾭洹??꾪븿
    private bool lastRandomMapSpawn = false; 
    private bool endMapSpawn = false;


    private int spawnedCount;  //留?湲몄씠 ?쒗븳
    public float ExitPointXPosition; //移대찓?? ?덇컻 ?쒗븳 ?꾪븿
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
        SpawnedIndex = index; //?쒕뜡 ?앹꽦???덇컻 ?꾩튂瑜?諛쏆븘?ㅺ린 ?꾪븿 0???? 1???? 2媛 ?꾨옒
    }


    void Update()
    {
        if (SpawnedIndex == 0 || SpawnedIndex == 1)
        {
            // ?ㅻⅨ履??대룞 諛??ㅽ룿
            if (!canSpawnRight && player.transform.position.x > startSpawnOffset)
            {
                canSpawnRight = true;
            }

            if (!lastRandomMapSpawn && canSpawnRight && player.transform.position.x + 20f > nextSpawnDistanceRight - tileLength /2f)
            {
                MapXSpawnToRight();
            }

            // ?쇱そ ?대룞 諛??ㅽ룿
            if (!canSpawnLeft && player.transform.position.x < -startSpawnOffset)
            {
                canSpawnLeft = true;
            }

            if (!lastRandomMapSpawn && canSpawnLeft && player.transform.position.x - 20f < nextSpawnDistanceLeft + tileLength /2f)
            {
                MapXSpawnToLeft();
            }
        }

        // 留?湲몄씠
        int mapLength = 0;
        if (!mapLengthLogged) // ?붾쾭洹?濡쒓렇????踰덈쭔 異쒕젰
        {
            mapLengthLogged = true;
        }


        if (!endMapSpawn)
        {
            if (mapLength == 1)
            {//?됯퇏 ?대━?????22~25珥?
                if (spawnedCount >= 4 + DreamManager.Instance.SlapNum)
                {
                    LimitMapSpawning();
                }
            }

            else if (mapLength >= 2)
            {//?됯퇏 ?대━?????40~45珥?
                if (spawnedCount >= 6 + DreamManager.Instance.SlapNum)
                {
                    LimitMapSpawning();
                }
            }
        }
    }
    

    void MapXSpawnToRight() // ?ㅻⅨ履쎌쑝濡?留??앹꽦
    {
        int randomint = UnityEngine.Random.Range(0, mapList.Count - 1);
        Vector3 spawnPos = new Vector3(nextSpawnDistanceRight, groundY, 0f);
        Instantiate(mapList[randomint], spawnPos, Quaternion.identity);
        nextSpawnDistanceRight += tileLength;

        spawnedCount++; // 留??앹꽦 ??利앷?


    }

    void MapXSpawnToLeft() // ?쇱そ?쇰줈 留??앹꽦
    {
        int randomint = UnityEngine.Random.Range(0, mapList.Count - 1);
        Vector3 spawnPos = new Vector3(nextSpawnDistanceLeft, groundY, 0f);
        Instantiate(mapList[randomint], spawnPos, Quaternion.identity);
        nextSpawnDistanceLeft -= tileLength;

        spawnedCount++; // 留??앹꽦 ??利앷?

    }

    //^^^^ 湲곕낯 留??앹꽦 濡쒖쭅




    void LimitMapSpawning()
    {
        endMapSpawn = true;    //?덉텧援??앹꽦
        lastRandomMapSpawn = true;  //洹몃쭔 ?쒕뜡 ?앹꽦, 留덉?留??쒕뜡留??ㅽ룿??
        SpawnExit();
    }
        


    void SpawnExit() //?덉텧
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


            // ?꾨━???덉쓽 "ExitDoor"瑜?李얘린
            Transform exitDoor = spawnedLastMap.transform.Find("ExitDoor");

            if (exitDoor != null)
            {
                ExitPointXPosition = exitDoor.position.x;
            }
        }
    }
}



//^^^^ 留듦만??議곗젅 濡쒖쭅



