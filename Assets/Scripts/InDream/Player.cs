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
            transform.localScale = new Vector3(1/2, 1/2, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-1/2, 1/2, 1);

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
            isJump = true;
            rigid.AddForce(Vector3.up*jumpPower, ForceMode2D.Impulse);
        }
    }
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.name == "Ground" || other.gameObject.name == "Ground1")
        {
            isJump = false;
        }
    }

    //장애물 충돌
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Obstacle")
        {
            Speed = Mathf.Sign(Speed) * 0.5f;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Obstacle")
        {
            Speed = Mathf.Clamp(Speed, -MaxSpeed, MaxSpeed);
        }
    }
}
