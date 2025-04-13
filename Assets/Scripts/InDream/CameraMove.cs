using System;
using UnityEngine;

public class Camera_move : MonoBehaviour
{
    public Transform target;
    public float speed;

//카메라 가동 범위 제한
    public Vector2 center; //범위 중심
    public Vector2 size; //범위 크기
    float height; //카메라 높이
    float width; //카메라 너비

    void Start()
    {
        height = Camera.main.orthographicSize;
        width =  height * Screen.width / Screen.height;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; //범위 붉게
        Gizmos.DrawWireCube(center, size);
    }


    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

        //제한 영역 범위
        float lx = size.x * 0.5f - width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + center.x, lx + center.x);

        float ly = size.y * 0.5f - height;
        float clampY = Mathf.Clamp(transform.position.y, -ly + center.y, ly + center.y);
    }
}