using UnityEngine;

public class FogMovement : MonoBehaviour
{
    [SerializeField] private float Xvelocity;
    [SerializeField] private float Yvelocity;
    private int SpawnedIndex;

    Vector3 target1 = new Vector3(210, 1, -5);
    Vector3 target2 = new Vector3(-210, 1, -5);
    Vector3 target3 = new Vector3(0, 70, -5);


    void Start()
    {
       
    }
    public void SetIndex(int index)
    {
        SpawnedIndex = index;
    }

    void Update()
    {
        if (SpawnedIndex == 0 ) //왼쪽 -> 오른쪽
            transform.position = Vector3.MoveTowards(transform.position, target1, Xvelocity);

        else if (SpawnedIndex == 1 ) //오른쪽 -> 왼쪽
            transform.position = Vector3.MoveTowards(transform.position, target2, Xvelocity);

        else if (SpawnedIndex == 2 ) //아래 -> 위쪽
            transform.position =Vector3.MoveTowards(transform.position, target3, Yvelocity);

    }
}
