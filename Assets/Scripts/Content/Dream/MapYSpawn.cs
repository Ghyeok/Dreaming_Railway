using UnityEngine;
using System.Collections.Generic;

public class MapYSpawn : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> mapList;

    private float nextSpawnY = 0f;
    private float cameraHeight;

    private bool canSpawnToUp = true;

    private int spawnedCount;
    private bool endMapSpawn = false;
    public float ExitPointYPosition;
    private FogOrigin origin;
    private SubwayData _subway;

    void Start()
    {
        _subway = GameDataManager.Instance.Subway;

        if (origin == FogOrigin.FromLeft || origin == FogOrigin.FromRight)
            canSpawnToUp = false;

        if (origin == FogOrigin.FromBottom)
        {
            cameraHeight = Camera.main.orthographicSize * 1.5f;

            for (int i = 0; i < 3; i++)
                MapYSpawnToUp();
        }
    }

    public void SetOrigin(FogOrigin fogOrigin)
    {
        origin = fogOrigin;
    }

    void Update()
    {
        if (player.transform.position.y + cameraHeight > nextSpawnY)
            MapYSpawnToUp();

        if (!endMapSpawn)
        {
            int mapLength = _subway.GetDreamMapLength();
            if (mapLength == 1)
            {
                if (spawnedCount >= 15 + _subway.SlapNum)
                    LimitMapSpawning();
            }
            else if (mapLength >= 2)
            {
                if (spawnedCount >= 22 + _subway.SlapNum)
                    LimitMapSpawning();
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
            nextSpawnY += 5f;
            spawnedCount++;
        }
    }

    void LimitMapSpawning()
    {
        endMapSpawn = true;
        canSpawnToUp = false;
        SpawnExit();
    }

    void SpawnExit()
    {
        GameObject spawnedLastMap = Instantiate(mapList[10], new Vector3(0, nextSpawnY, 0), Quaternion.identity);

        Transform exitDoor = spawnedLastMap.transform.Find("ExitDoor");
        if (exitDoor != null)
            ExitPointYPosition = exitDoor.position.y;
    }
}