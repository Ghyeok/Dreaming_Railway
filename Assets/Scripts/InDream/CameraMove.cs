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

        //카메라의 새로운 Y 위치를 계산(내려가지 않도록 제한)
        float newY = Mathf.Max(desiredPosition.y, minY);

        // 부드럽게 따라가되, Y축은 제한된 값으로 설정
        transform.position = Vector3.Lerp(transform.position, new Vector3(desiredPosition.x, newY, -10f), speed * Time.deltaTime);
    }

}