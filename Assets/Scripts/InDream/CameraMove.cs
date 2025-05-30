using System;
using UnityEngine;

public class Camera_move : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float minY = 1f;



    void Start()
    {
        
    }

    void Update()
    {
        //원하는 목표 설정(타깃 위치)
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -10f);

        //카메라의 새로운 Y 위치를 계산(게임 처음 시작 시 내려가지 않도록 제한)
        float newY = Mathf.Max(desiredPosition.y, minY);

        //카메라가 x축도 벗어나지 않도록(엔딩시)
        float newX;
        if (Mathf.Abs(MapXSpawn.Instance.ExitPointXPosition) < 0.01f)
        {
            newX = desiredPosition.x; // 제한 없음
        }
        else
        {
            newX = Mathf.Clamp(desiredPosition.x, -Mathf.Abs(MapXSpawn.Instance.ExitPointXPosition), Mathf.Abs(MapXSpawn.Instance.ExitPointXPosition));
        }
        // 부드럽게 따라가되, Y축은 제한된 값으로 설정
        transform.position = Vector3.Lerp(transform.position, new Vector3(newX, newY, -10f), speed * Time.deltaTime);
    }

}