using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public GameObject objectToSpawn;
    private Vector3 finalPosition;
    private bool hasSpawnedFinalObject = false;

    void Start()
    {
        }

    void Update()
    {
        /*  
        if ( Manager(파일명).Instance.(깨어있던 시간) < 50 && !hasSpawnedFinalObject )
        {
            Vector3 finalPosition = Vector3.zero;

            if (canSpawnRight)
            {//탈출구 생성 위치=(생성된 타일 개수*타일길이+-5,y,0f)
               finalPosition = new Vector3((10 +  2*Manager.Instance.(뺨 때린 횟수))*(MapXSpawn.Instance.tileLength) - 5f,MapXSpawn.Instance.groundY, 0f);
            }

            if (canSpawnLeft)
            {
                finalPosition = new Vector3((-(10 + 2*Manager.Instance.(뺨 때린 횟수))*(MapXSpawn.Instance.tileLength) + 5f,MapXSpawn.Instance.groundY, 0f);
            }



        else if ( Manager.Instance.(깨어잇던 시간) >= 50  && !hasSpawnedFinalObject )
        { 
            Vector3 finalPosition = Vector3.zero;

            if (canSpawnRight)
            {
               finalPosition = new Vector3((18 +  2*Manager.Instance.(뺨 때린 횟수))*(MapXSpawn.Instance.tileLength) - 5f,MapXSpawn.Instance.groundY, 0f);
            }

            if (canSpawnLeft)
            {
                finalPosition = new Vector3(-(18 + 2*Manager.Instance.(뺨 때린 횟수))*(MapXSpawn.Instance.tileLength) + 5f,MapXSpawn.Instance.groundY, 0f);
            }


        if (objectToSpawn != null)
        {
               Instantiate(objectToSpawn, finalPosition, Quaternion.identity);
               hasSpawnedFinalObject = true;
        }
    */

    }
}
