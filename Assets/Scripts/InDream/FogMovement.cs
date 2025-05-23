using UnityEngine;

public class FogMovement : MonoBehaviour
{
    [SerializeField] private float maxXVelocity;
    [SerializeField] private float maxYVelocity;
    [SerializeField] private float acceleration = 1f;
    private int SpawnedIndex;
    private float currentXVelocity = 0f;
    private float currentYVelocity = 0f;

    private bool hasReachedTarget = false;


    //안개가 도달할 마지막 지점(수정필요)
    Vector3 target1 = new Vector3(300, 1, -5);
    Vector3 target2 = new Vector3(-300, 1, -5);
    Vector3 target3 = new Vector3(0, 100, -5);


    void Start()
    {
       
    }
    public void SetIndex(int index)
    {
        SpawnedIndex = index; //랜덤 생성된 안개 위치를 받아오기 위함
    }

    void Update()
    {

        if (hasReachedTarget) return; // 이미 도착했으면 아무 것도 하지 않음

        // 가속 적용
        if (SpawnedIndex == 0 || SpawnedIndex == 1)
            currentXVelocity = Mathf.Min(currentXVelocity + acceleration * Time.deltaTime, maxXVelocity);
        else if (SpawnedIndex == 2)
            currentYVelocity = Mathf.Min(currentYVelocity + acceleration * Time.deltaTime, maxYVelocity);


        if (SpawnedIndex == 0) //왼쪽 -> 오른쪽
            transform.position = Vector3.MoveTowards(transform.position, target1, currentXVelocity * Time.deltaTime);
            if (Vector3.Distance(transform.position, target1) < 0.01f)
            {
                Debug.Log("게임오버! (오른쪽 도착)");
                hasReachedTarget = true;
            }

        else if (SpawnedIndex == 1) //오른쪽 -> 왼쪽
            transform.position = Vector3.MoveTowards(transform.position, target2, currentXVelocity * Time.deltaTime);
            if (Vector3.Distance(transform.position, target2) < 0.01f)
            {
                Debug.Log("게임오버! (왼쪽 도착)");
                hasReachedTarget = true;
            }

        else if (SpawnedIndex == 2 ) //아래 -> 위쪽
            transform.position =Vector3.MoveTowards(transform.position, target3, currentYVelocity * Time.deltaTime);
            if (Vector3.Distance(transform.position, target3) < 0.01f)
            {
                Debug.Log("게임오버! (위 도착)");
                hasReachedTarget = true;
            }  
    }
}



/*



private void OnCollisionEnter2D(Collision2D collision) 
    {// 탈출구랑 닿았을 때
        if (collision.collider.CompareTag("ExitDoor"))
        {
            Debug.Log("게임오버!");
        }
    }











//어둠 이동 속도
if ( (Manager.Instance.(깨어있던 시간) < 100) )
{
    Xvelocity = Xvelocity*0.8;
    Yvelocity = Yvelocity*0.8;
}

else if( 100 <= (Manager.Instance.(깨어있던 시간) <= 125) )
{    
    Xvelocity = Xvelocity*0.9;
    Yvelocity = Yvelocity*0.9;
}

else
{    
    Xvelocity = Xvelocity*1.0;
    Yvelocity = Yvelocity*1.0;
}


*/
