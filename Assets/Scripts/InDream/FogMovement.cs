using UnityEngine;
using System;


public class FogMovement : MonoBehaviour
{
    [SerializeField] private float maxXVelocity;
    [SerializeField] private float maxYVelocity;
    [SerializeField] private float acceleration = 1f;
    
   
    private int SpawnedIndex;


    private float currentXVelocity = 0f;
    private float currentYVelocity = 0f;

    private float realMaxXVelocity; // 실제 사용될 최대 X 속도
    private float realMaxYVelocity; // 실제 사용될 최대 Y 속도

    private SpriteRenderer fogRenderer;
    private bool IsGameOver = false;

   


    void Start()
    {
        //깨어있던 시간에 의거한 어둠 이동 속도 계산
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
        fogRenderer = GetComponent<SpriteRenderer>();
    }


    public void SetIndex(int index)
    {
        SpawnedIndex = index; //랜덤 생성된 안개 위치를 받아오기 위함 0이 왼, 1이 오, 2가 아래
    }


    void Update()
    {
        if (IsGameOver) return;

        //가속 이동동
        currentXVelocity = Mathf.Min(currentXVelocity + acceleration * Time.deltaTime, realMaxXVelocity);
        currentYVelocity = Mathf.Min(currentYVelocity + acceleration * Time.deltaTime, realMaxYVelocity);

        // 방향에 따라 이동
        if (SpawnedIndex == 0) // 오른쪽 → 왼쪽
        {
            transform.position += Vector3.right * currentXVelocity * Time.deltaTime;
        }
        else if (SpawnedIndex == 1) // 왼쪽 → 오른쪽
        {
            transform.position += Vector3.left * currentXVelocity * Time.deltaTime;
        }
        else if (SpawnedIndex == 2) // 아래 → 위
        {
            transform.position += Vector3.up * currentYVelocity * Time.deltaTime;
        }

        // 카메라 화면 덮힘 판정
        if (IsFogCoveringScreen())
        {
            IsGameOver = true;
            Debug.Log("게임오버!");
        }
    }


    bool IsFogCoveringScreen()
    {
        float margin = 5f; //여유 범위

        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 5)) - new Vector3(margin, margin, 0);
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 5)) + new Vector3(margin, margin, 0);
        Bounds fogBounds = fogRenderer.bounds;

        //결과 반환, 안개가 화면을 모두 덮었다면
        return fogBounds.Contains(bottomLeft) && fogBounds.Contains(topRight);
    }
}