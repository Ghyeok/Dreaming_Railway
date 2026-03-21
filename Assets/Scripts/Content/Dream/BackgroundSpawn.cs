using UnityEngine;

public class BackgroundSpawn : MonoBehaviour
{
    public Transform player; 
    public GameObject Background;  
    public float tileWidth;  // 배경 너비
    public float tileHeight; //배경 높이

    private float lastSpawnX;
    private float lastSpawnY;
    private float nextSpawnPoint;
    private float nextSpawnPointToUp;

    void Start()
    {

        lastSpawnX = player.transform.position.x;
        lastSpawnY = 9f;
        nextSpawnPoint = tileWidth;
        nextSpawnPointToUp = tileHeight;
    }


    void Update()
    {
        // 플레이어가 일정 거리 이상 이동했는지(좌우)
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



        // 플레이어가 일정 거리 이상 이동했는지(위)
        if (player.transform.position.y + 5f >= nextSpawnPointToUp - tileHeight / 2f)
        { //(플레이어 위치)+10가 새로 생기는 배경의 이어지는 부분보다 클때
            Vector3 spawnPosition1;
            Vector3 spawnPosition2;
            Vector3 spawnPosition3;

            if (player.position.y > lastSpawnY)
            {
                // 3칸 배경 생성
                spawnPosition1 = new Vector3(0f, lastSpawnY + tileHeight, 3f);
                spawnPosition2 = new Vector3(0f + tileWidth, lastSpawnY + tileHeight, 3f);
                spawnPosition3 = new Vector3(0f - tileWidth, lastSpawnY + tileHeight, 3f);

                Instantiate(Background, spawnPosition1, Quaternion.identity, transform);
                Instantiate(Background, spawnPosition2, Quaternion.identity, transform);
                Instantiate(Background, spawnPosition3, Quaternion.identity, transform);
                lastSpawnY = spawnPosition1.y;
                nextSpawnPointToUp += tileHeight;
            }
            
        }
    }
}


