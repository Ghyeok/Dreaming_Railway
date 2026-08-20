using System;
using UnityEngine;

public class FogMovement : MonoBehaviour
{
    [SerializeField] private float maxXVelocity;
    [SerializeField] private float maxYVelocity;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float delayAfterCover;
    [SerializeField] private float margin;

    private bool isCounting = false;
    private float coverTimer = 0f;

    private FogOrigin origin;

    private float currentXVelocity = 0f;
    private float currentYVelocity = 0f;

    private float realMaxXVelocity;
    private float realMaxYVelocity;

    private SpriteRenderer fogRenderer;
    private bool IsGameOver = false;
    private bool gameOverTriggered = false;
    private Camera_move cameraMoveScript;
    private Player player;
    private bool playerMoveStarted = false;

    public static event Action OnDreamGameOver;

    void Start()
    {
        int awakeTime = GameDataManager.Instance.Subway.GetDreamMapLength();
        float speedConstant = 1f;

        if (awakeTime <= 2)
            speedConstant = 0.8f;
        else if (awakeTime == 3)
            speedConstant = 0.9f;

        realMaxXVelocity = maxXVelocity * speedConstant;
        realMaxYVelocity = maxYVelocity * speedConstant;
    }

    void Awake()
    {
        fogRenderer = GetComponent<SpriteRenderer>();
        if (Camera.main != null)
            cameraMoveScript = Camera.main.GetComponent<Camera_move>();
        player = FindFirstObjectByType<Player>();
    }

    public void SetOrigin(FogOrigin fogOrigin)
    {
        origin = fogOrigin;
    }

    void Update()
    {
        if (!playerMoveStarted && player != null)
        {
            if (player.Speed != 0 || !player.isGrounded)
                playerMoveStarted = true;
            else
                return;
        }

        if (IsGameOver) return;

        currentXVelocity = Mathf.Min(currentXVelocity + acceleration * Time.deltaTime, realMaxXVelocity);
        currentYVelocity = Mathf.Min(currentYVelocity + acceleration * Time.deltaTime, realMaxYVelocity);

        if (origin == FogOrigin.FromLeft)
            transform.position += Vector3.right * currentXVelocity * Time.deltaTime;
        else if (origin == FogOrigin.FromRight)
            transform.position += Vector3.left * currentXVelocity * Time.deltaTime;
        else if (origin == FogOrigin.FromBottom)
            transform.position += Vector3.up * currentYVelocity * Time.deltaTime;

        if (!isCounting && IsFogCoveringScreen())
        {
            isCounting = true;
            coverTimer = 0f;
        }

        if (isCounting)
        {
            coverTimer += Time.deltaTime;

            if (coverTimer >= delayAfterCover)
            {
                IsGameOver = true;

                if (!gameOverTriggered)
                {
                    OnDreamGameOver?.Invoke();
                    cameraMoveScript.enabled = false;
                    gameOverTriggered = true;
                }
            }
        }
    }

    bool IsFogCoveringScreen()
    {
        float z = transform.position.z;
        float depth = Mathf.Abs(Camera.main.transform.position.z - z);

        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, depth)) - new Vector3(margin, margin, 0);
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, depth)) + new Vector3(margin, margin, 0);

        bottomLeft.z = z;
        topRight.z = z;

        Bounds fogBounds = fogRenderer.bounds;
        return fogBounds.Contains(bottomLeft) && fogBounds.Contains(topRight);
    }
}