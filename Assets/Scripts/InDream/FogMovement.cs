using UnityEngine;
using System;


public class FogMovement : MonoBehaviour
{
    [SerializeField] private float maxXVelocity;
    [SerializeField] private float maxYVelocity;
    [SerializeField] private float acceleration = 1f;
    
    [SerializeField] private float tempTargetXDistance = 300f; // X축 이동 안개용 임시 목표 거리
    [SerializeField] private float tempTargetYDistance = 100f; // Y축 이동 안개용 임시 목표 거리

    private int SpawnedIndex;
    private Vector3 initialPosition;

    private float currentXVelocity = 0f;
    private float currentYVelocity = 0f;

    private float realMaxXVelocity; // 실제 사용될 최대 X 속도
    private float realMaxYVelocity; // 실제 사용될 최대 Y 속도

   


    void Start()
    {
        //깨어있던 시간에 의거한 어둠 이동 속도
        int awakeTime = SubwayGameManager.Instance.SetDreamMapLengthByAwakenTime();
        float speedConstant = 1f;

        if (awakeTime <= 2)
        {
            speedConstant = 0.8f;
        }

        else if (awakeTime == 3)
        {
            speedConstant = 0.9f;
        }

        else if (awakeTime == 4)
        {
            speedConstant = 1f;
        }

        realMaxXVelocity = maxXVelocity * speedConstant;
        realMaxYVelocity = maxYVelocity * speedConstant;

    }

    void Awake()
    {
        initialPosition = transform.position;
    }



    public void SetIndex(int index)
    {
        SpawnedIndex = index; //랜덤 생성된 안개 위치를 받아오기 위함 0이 왼, 1이 오, 2가 아래
    }


    void Update()
    {
        Vector3 targetPosition;//목표지점

        currentXVelocity = Mathf.Min(currentXVelocity + acceleration * Time.deltaTime, realMaxXVelocity);
        currentYVelocity = Mathf.Min(currentYVelocity + acceleration * Time.deltaTime, realMaxYVelocity);


        if (SpawnedIndex == 0 || SpawnedIndex == 1) //안개가 좌우 이동
        {

            if (MapXSpawn.Instance != null && MapXSpawn.Instance.ExitPointXPosition != 0f)
            {//목적지 있을때
                targetPosition = new Vector3(MapXSpawn.Instance.ExitPointXPosition, transform.position.y, transform.position.z);
            }
            else
            {//목적지 없을떄
                float tempTargetX = initialPosition.x + (SpawnedIndex == 0 ? tempTargetXDistance : -tempTargetXDistance);
                targetPosition = new Vector3(tempTargetX, transform.position.y, transform.position.z);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentXVelocity * Time.deltaTime);
        }


        else if (SpawnedIndex == 2) //안개가 위 이동
        {
            if (MapYSpawn.Instance != null && MapYSpawn.Instance.ExitPointYPosition != 0f)
            {
                targetPosition = new Vector3(transform.position.x, MapYSpawn.Instance.ExitPointYPosition, transform.position.z);
            }
            else
            {
                float tempTargetY = transform.position.y + tempTargetYDistance;
                targetPosition = new Vector3(transform.position.x, tempTargetY, transform.position.z);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentYVelocity * Time.deltaTime);
        }

    }


    void OnCollisionEnter2D(Collision2D collision)
    {// 탈출구랑 닿았을 때
        if (collision.collider.CompareTag("ExitDoor"))
        {
            Debug.Log("게임오버!");
        }
    }
}

