using UnityEngine;

public class Camera_move : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float minY = 1f;
    private FogOrigin origin;
    private float initialX;
    private MapXSpawn mapXSpawn;

    public void Init(MapXSpawn spawner)
    {
        mapXSpawn = spawner;
    }

    void Start()
    {
        if (target != null)
            initialX = target.position.x;
    }

    public void SetOrigin(FogOrigin fogOrigin)
    {
        origin = fogOrigin;
    }

    void Update()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -10f);
        float newY = Mathf.Max(desiredPosition.y, minY);
        float newX = desiredPosition.x;

        if (origin == FogOrigin.FromLeft)
            newX = Mathf.Max(desiredPosition.x, initialX);
        else if (origin == FogOrigin.FromRight)
            newX = Mathf.Min(desiredPosition.x, initialX);
        else if (origin == FogOrigin.FromBottom)
            newX = Mathf.Clamp(desiredPosition.x, -6f, 6f);

        transform.position = Vector3.Lerp(transform.position, new Vector3(newX, newY, -10f), speed * Time.deltaTime);
    }
}