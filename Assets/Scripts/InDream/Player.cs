using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{ 
    [SerializeField] private float acceleration;  
    [SerializeField] private float MaxSpeed;      
    [SerializeField] private Rigidbody2D rigid;
   
    private float Speed = 2.5f;
    public float jumpPower;
    public float UImoveX = 2.5f;
    public bool isJump = false;
    public float minSpeed = 1.75f;
    private bool isInObstacle = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
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
    
    void Footstep()
    {
        while ((Speed != 0) && (isJump = false))
        {
            SoundManager.Instance.Footstep1SFX();
            SoundManager.Instance.Footstep2SFX();

            if ((Speed == 0)  ||  (isJump = true))
            {
                break;
            }       
        }
    }   
}

