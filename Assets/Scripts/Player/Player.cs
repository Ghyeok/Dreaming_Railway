using UnityEngine;

public class Player : MonoBehaviour
{ 
    [SerializeField] [Range(1f, 20f)] float moveSpeed = 5f;
    [SerializeField] [Range(1f, 20f)] float jumpForce = 5f;
    private Rigidbody2D rb;

    private bool moveRight, moveLeft;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //좌우 움직이기
    void Update()
    {
        if (moveRight)
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        else if (moveLeft)
       
         transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

    public void StartMoveRight() => moveRight = true;
    public void StopMoveRight() => moveRight = false;

    public void StartMoveLeft() => moveLeft = true;
    public void StopMoveLeft() => moveLeft = false;

    //점프
    public void Jump()
    {   
      if (Mathf.Approximately(rb.linearVelocity.y, 0))
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
}

