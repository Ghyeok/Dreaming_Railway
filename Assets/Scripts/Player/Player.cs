using UnityEngine;

public class Player : MonoBehaviour
{ 
    [SerializeField] [Range(1f, 20f)] float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rigid;

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
    {
        float moveX = Input.GetAxis("Horizontal");

        if (moveRight)
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        else if (moveLeft)
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
   
     rigid.linearVelocity = new Vector2(moveX*moveSpeed, rigid.linearVelocity.y);
    }

    public void Jump()
    {
        if (!isJump)
            {
                isJump = true;
                rigid.AddForce(Vector3.up*jumpPower, ForceMode2D.Impulse);
            }
        }

    public void StartMoveRight() => moveRight = true;
    public void StopMoveRight() => moveRight = false;
    public void StartMoveLeft() => moveLeft = true;
    public void StopMoveLeft() => moveLeft = false;

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.name.Equals("Ground"))
        {
            isJump = false;
        }
    }

}
