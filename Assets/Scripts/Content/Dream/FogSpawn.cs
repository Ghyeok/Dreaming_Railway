using System;
using UnityEngine;

public class FogSpawn : MonoBehaviour
{
    public GameObject Fog;
    public Vector3[] spawnPositions = new Vector3[3];

    public static event Action<FogOrigin, FogMovement> OnOriginDecided;

    void Start()
    {
        SpawnRandomPosition();
    }

    void SpawnRandomPosition()
    {
        int randomIndex = UnityEngine.Random.Range(0, spawnPositions.Length);
        FogOrigin origin = (FogOrigin)randomIndex;

        if (origin == FogOrigin.FromLeft)
            transform.rotation = Quaternion.identity;
        else if (origin == FogOrigin.FromRight)
            transform.rotation = Quaternion.Euler(0, 0, 180);
        else if (origin == FogOrigin.FromBottom)
            transform.rotation = Quaternion.Euler(0, 0, 90);

        GameObject fogClone = Instantiate(Fog, spawnPositions[randomIndex], transform.rotation);
        FogMovement fogMovement = fogClone.GetComponent<FogMovement>();

        OnOriginDecided?.Invoke(origin, fogMovement);
    }
}