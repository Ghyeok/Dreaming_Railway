using UnityEngine;
using System.Collections.Generic;

public class MapXSpawn : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> mapList;

    public float groundY;
    public float tileLength;
    public float startSpawnOffset;

    private float nextSpawnDistanceRight;
    private float nextSpawnDistanceLeft;

    public bool canSpawnRight = false;
    public bool canSpawnLeft = false;

    private bool lastRandomMapSpawn = false;
    private bool endMapSpawn = false;

    private int spawnedCount;
    public float ExitPointXPosition;
    private FogOrigin origin;
    private SubwayData _subway;

    void Start()
    {
        _subway = GameDataManager.Instance.Subway;

        nextSpawnDistanceRight = tileLength;
        nextSpawnDistanceLeft = -tileLength;
        spawnedCount = 0;

        if (origin == FogOrigin.FromBottom)
        {
            canSpawnRight = false;
            canSpawnLeft = false;
        }
    }

    public void SetOrigin(FogOrigin fogOrigin)
    {
        origin = fogOrigin;
    }

    void Update()
    {
        if (origin == FogOrigin.FromLeft || origin == FogOrigin.FromRight)
        {
            if (!canSpawnRight && player.transform.position.x > startSpawnOffset)
                canSpawnRight = true;

            if (!lastRandomMapSpawn && canSpawnRight && player.transform.position.x + 20f > nextSpawnDistanceRight - tileLength / 2f)
                MapXSpawnToRight();

            if (!canSpawnLeft && player.transform.position.x < -startSpawnOffset)
                canSpawnLeft = true;

            if (!lastRandomMapSpawn && canSpawnLeft && player.transform.position.x - 20f < nextSpawnDistanceLeft + tileLength / 2f)
                MapXSpawnToLeft();
        }

        if (!endMapSpawn)
        {
            int mapLength = _subway.GetDreamMapLength();
            if (mapLength == 1)
            {
                if (spawnedCount >= 4 + _subway.SlapNum)
                    LimitMapSpawning();
            }
            else if (mapLength >= 2)
            {
                if (spawnedCount >= 6 + _subway.SlapNum)
                    LimitMapSpawning();
            }
        }
    }

    void MapXSpawnToRight()
    {
        int randomint = UnityEngine.Random.Range(0, mapList.Count - 1);
        Vector3 spawnPos = new Vector3(nextSpawnDistanceRight, groundY, 0f);
        Instantiate(mapList[randomint], spawnPos, Quaternion.identity);
        nextSpawnDistanceRight += tileLength;
        spawnedCount++;
    }

    void MapXSpawnToLeft()
    {
        int randomint = UnityEngine.Random.Range(0, mapList.Count - 1);
        Vector3 spawnPos = new Vector3(nextSpawnDistanceLeft, groundY, 0f);
        Instantiate(mapList[randomint], spawnPos, Quaternion.identity);
        nextSpawnDistanceLeft -= tileLength;
        spawnedCount++;
    }

    void LimitMapSpawning()
    {
        endMapSpawn = true;
        lastRandomMapSpawn = true;
        SpawnExit();
    }

    void SpawnExit()
    {
        Vector3 spawnPos = player.transform.position.x >= 0f
            ? new Vector3(nextSpawnDistanceRight, groundY, 0f)
            : new Vector3(nextSpawnDistanceLeft, groundY, 0f);

        GameObject spawnedLastMap = Instantiate(mapList[10], spawnPos, Quaternion.identity);

        Transform exitDoor = spawnedLastMap.transform.Find("ExitDoor");
        if (exitDoor != null)
            ExitPointXPosition = exitDoor.position.x;
    }
}