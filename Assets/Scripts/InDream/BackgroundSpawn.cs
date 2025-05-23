using UnityEngine;

public class BackgroundSpawn : MonoBehaviour
{
    public Transform player; 
    public GameObject Background;  
    public float tileWidth;  // 배경 너비

    private float lastSpawnX;
    private float nextSpawnDistance;

    void Start()
    {
        lastSpawnX = player.transform.position.x;
        nextSpawnDistance = tileWidth;
    }

    void Update()
    {
        // 플레이어가 일정 거리 이상 이동했는지
        if (Mathf.Abs(player.transform.position.x) + 15f >= nextSpawnDistance - tileWidth/2f)
        {
            Vector3 spawnPosition;

            if (player.position.x > lastSpawnX)
            {
                // 오른쪽에 배경 생성
                spawnPosition = new Vector3(lastSpawnX + tileWidth, 8f, 5f);
            }
            else
            {
                // 왼쪽에 배경 생성
                spawnPosition = new Vector3(lastSpawnX - tileWidth, 8f, 5f);
            }

            Instantiate(Background, spawnPosition, Quaternion.identity, transform);
            lastSpawnX = spawnPosition.x;
        }
    }
}


