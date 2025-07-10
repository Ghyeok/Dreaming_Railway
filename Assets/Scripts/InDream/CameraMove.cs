using System;
using UnityEngine;

public class Camera_move : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float minY = 1f;
    private int SpawnedIndex;
    private float initialX;



    void Start()
    {
        if (target != null)
        {
            initialX = target.position.x;
        }
    }

    public void SetIndex(int index)
    {
        SpawnedIndex = index; //랜덤 생성된 안개 위치를 받아오기 위함 0이 왼, 1이 오, 2가 아래
    }

    void Update()
    {
        if (target == null) return;

        //원하는 목표 설정(타깃 위치)
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -10f);

        //카메라의 새로운 Y 위치를 계산(게임 처음 시작 시 내려가지 않도록 제한)
        float newY = Mathf.Max(desiredPosition.y, minY);

        //카메라가 x축 벗어나지 않도록(엔딩시)
        float newX = desiredPosition.x;

        // 어둠 생성 방향에 따라 카메라 이동 제한
        if (SpawnedIndex == 0) // 왼 -> 오 /플레이어 오른쪽으로
        {
            newX = Mathf.Max(desiredPosition.x, initialX);
        }
        else if (SpawnedIndex == 1) // 오 -> 왼 
        {
            newX = Mathf.Min(desiredPosition.x, initialX);
        }
        else
        {
            if (Mathf.Abs(MapXSpawn.Instance.ExitPointXPosition) >= 0.01f)
            {
                newX = Mathf.Clamp(desiredPosition.x, -Mathf.Abs(MapXSpawn.Instance.ExitPointXPosition), Mathf.Abs(MapXSpawn.Instance.ExitPointXPosition));
            }
        }

        // 부드럽게 따라가되, Y축은 제한된 값으로 설정
        transform.position = Vector3.Lerp(transform.position, new Vector3(newX, newY, -10f), speed * Time.deltaTime);
    }
}