using UnityEngine;

public class Player : MonoBehaviour
{ 
   // [SerializeField] [Range(1f, 20f)] float moveSpeed = 5f;
    [SerializeField] private float acceleration;  
    [SerializeField] private float MaxSpeed = 8f;       
    [SerializeField] private Rigidbody2D rigid;
    private float Speed = 0f;

    public float jumpPower;
    public bool isJump = false;
    private bool moveRight, moveLeft;

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
        float moveX = Input.GetAxis("Horizontal");

    //UI로 이동
        if (moveRight) moveX = 1;
        else if (moveLeft) moveX = -1;

    //방향 전환 시
        if ((Speed > 0 && moveX < 0) || (Speed < 0 && moveX > 0))
            Speed = 0; //방향 바꾸면 속도 멈춤

    //방향 따라 플레이어 좌우 반전전
        if (moveX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-1, 1, 1);

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

//점프 함수
    public void Jump()
    {
        if (!isJump)
            {
                isJump = true;
                rigid.AddForce(Vector3.up*jumpPower, ForceMode2D.Impulse);
            }
        }

//UI 좌우 버튼을 위한..
    public void StartMoveRight() 
    {
        moveRight = true;
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void StopMoveRight() => moveRight = false;
    public void StartMoveLeft()
    {
        moveLeft = true;
        transform.localScale = new Vector3(-1, 1, 1);
    }
     
    public void StopMoveLeft() => moveLeft = false;

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.name.Equals("Ground"))
        {
            isJump = false;
        }
    }
}
