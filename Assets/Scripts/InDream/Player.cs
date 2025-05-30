using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



public class Player : MonoBehaviour
{ 
    [SerializeField] private float acceleration;  
    [SerializeField] private float MaxSpeed;      
    [SerializeField] private Rigidbody2D rigid;

//발소리를 위한
    private Vector2 lastFootstepPosition;
    private float distanceMovedSinceLastStep = 0f;
    [SerializeField] private float stepDistance; // 발소리가 나는 최소 거리
    
    public float Speed;
    public float jumpPower;
    private float UimoveX;    
    
    public float minSpeed;
    private bool isLeftFoot = true;
    public bool isJump = false;

    private bool isInObstacle = false;
    public bool wasMovingLastFrame = false;

    Animator MyAnimator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        MyAnimator = GetComponent<Animator>();
        Speed = 0f;
    }
    
    void Update() 
    {
        
        if (Input.GetKeyDown(KeyCode.Space)) //점프
        {
            Jump();
        }
        
    }
    
    void FixedUpdate() 
    {// UI 키와 키보드 입력을 동시에 처리(방향값 받기)
        float moveX = 0f;

        if (Input.GetKey(KeyCode.RightArrow) || UimoveX > 0) // 오른쪽 키 또는 UI 입력
            moveX = 1f;
        else if (Input.GetKey(KeyCode.LeftArrow) || UimoveX < 0) // 왼쪽 키 또는 UI 입력
            moveX = -1f;

        
        //방향 전환 시
        if ((Speed < 0 && moveX > 0) || (Speed > 0 && moveX < 0))
            Speed = -Speed/2; // 방향만 반전


        //방향 따라 플레이어 좌우 반전
        if (moveX > 0)
            transform.localScale = new Vector3(0.45f, 0.45f, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-0.45f, 0.45f, 1);


        if (moveX != 0)
        {//좌우 누르면
            if (!wasMovingLastFrame && Mathf.Abs(Speed) < 0.1f)
            {
            // 정지 상태 -> 처음 입력됨 -> 4부터 시작
                Speed = moveX * 4f;
                wasMovingLastFrame = true;
            }
            else
            {
                // 가속 적용
                Speed += moveX * acceleration * Time.fixedDeltaTime;
                Speed = Mathf.Clamp(Speed, -MaxSpeed, MaxSpeed);
            }

        }
    
        else //안 누르면 감속
        {
            Speed = Mathf.MoveTowards(Speed, 0, 3*acceleration * Time.fixedDeltaTime);

            if (Mathf.Abs(Speed) < 0.01f)
            {
                wasMovingLastFrame = false;
            }
        }


    //장애물 감속
        if (isInObstacle)
        {
            SoundManager.Instance.EnterFogSFX();
            float absCurrentSpeed = Mathf.Abs(Speed);
            float direction = Mathf.Sign(Speed);
            float decelerationRate = (absCurrentSpeed - minSpeed) / 2f; // 2초 동안 줄어들도록

    //(3초 후 최소속도 도달)
            float absSlowSpeed = Mathf.MoveTowards(absCurrentSpeed, minSpeed, decelerationRate * Time.fixedDeltaTime);
            Speed = direction * absSlowSpeed;
        }


        rigid.linearVelocity = new Vector2(Speed, rigid.linearVelocity.y);



    //발자국 소리
       if ( (Speed != 0) && (!isJump) ) //속도가 0이 아니고 점프하고 있지 않을 때
       {
        float moved = Vector2.Distance(transform.position, lastFootstepPosition);//움직인 거리
        distanceMovedSinceLastStep += moved; //마지막 발자국으로부터의 거리 갱신
            if (distanceMovedSinceLastStep >= stepDistance)//마지막 발자국으로부터의 거리가 특정 거리 이상일 때
            {
                if (isLeftFoot) //왼발 앞
                    SoundManager.Instance.Footstep1SFX();
                else
                    SoundManager.Instance.Footstep2SFX();//오른발 앞
                    isLeftFoot = !isLeftFoot;
                    distanceMovedSinceLastStep = 0f;//거리 초기화
                    lastFootstepPosition = transform.position;
            }
        }
        else
        {// 멈추면 위치 갱신
            lastFootstepPosition = transform.position;
        }
        //달리기 모션
        MyAnimator.SetBool("IsRunning", Speed != 0);
    }


//UI로의 이동
    public void StartMoveRight() 
    {
        UimoveX = 1;
    }
    public void StartMoveLeft()
    {
        UimoveX = -1;
    }
    public void StopMove()
    {
        UimoveX = 0f; //정지
    }
    

//점프 함수
    public void Jump()
    {
        if (!isJump)
        {
            SoundManager.Instance.JumpSFX();
            MyAnimator.SetBool("IsJumping", true);
            isJump = true;
            rigid.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) 
    {// 바닥 윗 표면에 착지할 때만
        if (collision.collider.CompareTag("Ground") && collision.contacts[0].normal.y > 0.8f)
        {
            SoundManager.Instance.LandSFX();
            MyAnimator.SetBool("IsJumping", false);
            isJump = false;
        }

        //꿈 속 탈출구랑 닿았을 때 씬 넘어가기기
        if (collision.collider.CompareTag("ExitDoor"))
        {
            SoundManager.Instance.ExitDreamSFX();
            SceneManager.LoadScene("TestSubwayScene"); 
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {   //장애물 감지
        if (other.CompareTag("Obstacle"))
        {
            isInObstacle = true;
        }
    }

    

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            isInObstacle = false;
        }
    }  

}

