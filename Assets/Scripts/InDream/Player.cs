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
    private bool isLeftFoot = true;

    private float Speed = 2.5f;
    public float jumpPower;
    public float UImoveX = 2.5f;
    public bool isJump = false;
    public float minSpeed = 1.75f;
    private bool isInObstacle = false;

    Animator MyAnimator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        MyAnimator = GetComponent<Animator>();
    }
    
    void Update() 
    {
        
        if (Input.GetKeyDown(KeyCode.Space)) //점프
        {
            Jump();
        }
        if ( (Mathf.Abs(transform.position.x) > 200) || (transform.position.y > 50) )
        {//씬 이동
            SoundManager.Instance.ExitDreamSFX();
            SceneManager.LoadScene("TestSubwayScene");
        }
    }
    
    void FixedUpdate() 
    {//키보드키
        float KeyboardmoveX = Input.GetAxis("Horizontal");

    //키보드 혹은 UI에서 방향값 받기
        float moveX = KeyboardmoveX != 0 ? KeyboardmoveX : UImoveX;

    //방향 전환 시
        if ((Speed < 0 && moveX > 0) || (Speed > 0 && moveX < 0))
            Speed = 0; //방향 바꾸면 속도 멈춤

    //방향 따라 플레이어 좌우 반전
        if (moveX > 0)
            transform.localScale = new Vector3(0.45f, 0.45f, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-0.45f, 0.45f, 1);

    //가속
        if (moveX != 0)
        {
            Speed += moveX * acceleration * Time.fixedDeltaTime;
            Speed = Mathf.Clamp(Speed, -MaxSpeed, MaxSpeed);
        }

    //감속
        else
        {
            Speed = Mathf.MoveTowards(Speed, 0, 3*acceleration * Time.fixedDeltaTime);
        }


    //장애물 감속
        if (isInObstacle)
        {
            SoundManager.Instance.EnterFogSFX();
            float absCurrentSpeed = Mathf.Abs(Speed);
            float direction = Mathf.Sign(Speed);
            float decelerationRate = (absCurrentSpeed - minSpeed) / 3f; // 3초 동안 줄어들도록

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
        UImoveX = 1;
    }
    public void StartMoveLeft()
    {
        UImoveX = -1;
    }
    public void StopMove()
    {
        UImoveX = 0f; //정지
    }

//점프 함수
    public void Jump()
    {
        if (!isJump)
        {
            SoundManager.Instance.JumpSFX();
            isJump = true;
            rigid.AddForce(Vector3.up*jumpPower, ForceMode2D.Impulse); 
        }
    }
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            SoundManager.Instance.LandSFX();
            isJump = false;
        }
    }

    //장애물 감지
    private void OnTriggerStay2D(Collider2D other)
    {
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

