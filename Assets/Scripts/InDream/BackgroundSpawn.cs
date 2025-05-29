using UnityEngine;

public class BackgroundSpawn : MonoBehaviour
{
    public Transform player; 
    public GameObject Background;  
    public float tileWidth;  // 배경 너비

    private float lastSpawnX;
    private float nextSpawnPoint;

    void Start()
    {

        lastSpawnX = player.transform.position.x;
        nextSpawnPoint = tileWidth;
    }


    void Update()
    {
        // 플레이어가 일정 거리 이상 이동했는지
        if (Mathf.Abs(player.transform.position.x) + 15f >= nextSpawnPoint - tileWidth / 2f)
        //(플레이어 위치)+15가 새로 생기는 배경의 이어지는 부분보다 클때
        {
            Vector3 spawnPosition;

            if (player.position.x > lastSpawnX)
            {
                // 오른쪽에 배경 생성
                spawnPosition = new Vector3(lastSpawnX + tileWidth, 9f, 3f);
            }
            else
            {
                // 왼쪽에 배경 생성
                spawnPosition = new Vector3(lastSpawnX - tileWidth, 9f, 3f);
            }

            Instantiate(Background, spawnPosition, Quaternion.identity, transform);
            lastSpawnX = spawnPosition.x;
            nextSpawnPoint += tileWidth;
        }
    }
}


