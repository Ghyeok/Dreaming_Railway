using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class MapYSpawn : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> mapList;

    private float nextSpawnY = 0.5f;
    private float cameraHeight;

    int randomint;  //타일 번호 랜덤 변수

    //맵 길이 제한
    private int spawnedCount;


    void Start()
    {
        cameraHeight = Camera.main.orthographicSize * 1f;

        //시작 시 3개 미리 생성
        for (int i = 0; i < 3; i++)
        {
            MapYSpawnToUp();
        }
    }

    void Update()
    {
        //카메라보다 위에 미리 맵이 없으면 생성
        if (player.transform.position.y + cameraHeight > nextSpawnY)
        {
           MapYSpawnToUp();
        }
    }

    void MapYSpawnToUp()
    {
        randomint = Random.Range(0, mapList.Count); //맵패턴 숫자만큼까지 랜덤으로
        GameObject selectedMap = mapList[randomint]; //하나 뽑기

        //고른 맵의 정보 읽기
        float mapHeight = GetPrefabHeight(selectedMap);

        //고른 맵 생성
        Instantiate(selectedMap, new Vector3(10f, nextSpawnY, 0), Quaternion.identity);

       //다음 생성 위치 설정
        nextSpawnY += mapHeight + 1.8f;

        // 맵 생성 수 증가
        spawnedCount++; 
    }


   //맵 프리팹 높이 총괄 계산
    float GetPrefabHeight(GameObject prefab)
   {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
    
        if (renderers.Length == 0)
            return 0f;

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer r in renderers)
        {//모든 렌더러를 포함하는 전체 bounds 생성
            bounds.Encapsulate(r.bounds); 
        }

        return bounds.size.y;
    }

}


 /* 

if ( Manager(파일명).Instance.(깨어있던 시간) < 50 )
{//평균 클리어 타임 22~25초
    if ( spawnedCount == 10 +  2*Manager.Instance.(뺨 때린 횟수) )
    {   
        canSpawnRight = false;
        canSpawnleft = false;
    }
  
else if ( Manager.Instance.(깨어잇던 시간) >= 50 )
//평균 클리어 타임 40~45초
    if ( spawnedCount == 18 + 2*Manager.Instance.(뺨 때린 횟수) )
    {   
        canSpawnRight = false;
        canSpawnleft = false;
    }


//^^^^ 맵길이 계산 로직


*/
