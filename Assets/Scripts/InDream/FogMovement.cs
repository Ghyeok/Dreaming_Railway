using UnityEngine;



public class FogMovement : MonoBehaviour
{
    [SerializeField] private float maxXVelocity;
    [SerializeField] private float maxYVelocity;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float delayAfterCover = 3f;
    [SerializeField] private float margin;

    private bool isCounting = false;
    //public static bool fadeOutStart = false; //기존 UI 페이드 아웃 위함
    private float coverTimer = 0f; //어둠 멈춤 딜레이


    private int SpawnedIndex;


    private float currentXVelocity = 0f;
    private float currentYVelocity = 0f;

    private float realMaxXVelocity; // 실제 사용될 최대 X 속도
    private float realMaxYVelocity; // 실제 사용될 최대 Y 속도

    private SpriteRenderer fogRenderer;
    private bool IsGameOver = false;
    private bool gameOverTriggered = false; //한 번만 페이딩 함수 호출 하려고




    void Start()
    {
        //깨어있던 시간에 의거한 어둠 이동 속도 계산
        int awakeTime = SubwayGameManager.Instance.SetDreamMapLengthByAwakenTime();
        float speedConstant = 1f;

        if (awakeTime <= 2)
        {
            speedConstant = 0.8f;
        }

        else if (awakeTime == 3)
        {
            speedConstant = 0.9f;
        }

        else if (awakeTime == 4)
        {
            speedConstant = 1f;
        }

        realMaxXVelocity = maxXVelocity * speedConstant;
        realMaxYVelocity = maxYVelocity * speedConstant;

    }

    void Awake()
    {
        fogRenderer = GetComponent<SpriteRenderer>();
    }


    public void SetIndex(int index)
    {
        SpawnedIndex = index; //랜덤 생성된 안개 위치를 받아오기 위함 0이 왼으로, 1이 오로, 2가 아래
    }

    void Update()
    {
        if (IsGameOver) return;

        //가속 이동
        currentXVelocity = Mathf.Min(currentXVelocity + acceleration * Time.deltaTime, realMaxXVelocity);
        currentYVelocity = Mathf.Min(currentYVelocity + acceleration * Time.deltaTime, realMaxYVelocity);

        // 방향에 따라 이동
        if (SpawnedIndex == 0) // (어둠이) 왼 -> 오 / 플레이어는 오른쪽으로 이동
        {
            transform.position += Vector3.right * currentXVelocity * Time.deltaTime;
        }
        else if (SpawnedIndex == 1) // 오 -> 왼 / 플레이어는 왼쪽으로 이동
        {
            transform.position += Vector3.left * currentXVelocity * Time.deltaTime;
        }
        else if (SpawnedIndex == 2) // 아래 -> 위
        {
            transform.position += Vector3.up * currentYVelocity * Time.deltaTime;
        }


        // 덮었으면 타이머 시작
        if (!isCounting && IsFogCoveringScreen())
        {
            isCounting = true;
            //fadeOutStart = true; //-> 게임오버UI 스크립트에서 전달
            Debug.Log("안개 도착, 게임오버!");
            coverTimer = 0f;
        }


        if (isCounting)
        {//어둠 흰 선 때문에 게임 오버 후 일정 시간 후 멈추도록
            coverTimer += Time.deltaTime;

            if (coverTimer >= delayAfterCover)
            {
                IsGameOver = true;

                if (IsGameOver && !gameOverTriggered)
                {
                    UI_GameOverPopup popup = UIManager.Instance.ShowPopupUI<UI_GameOverPopup>("UI_GameOverPopup");
                    CanvasGroup nonUI = GameObject.Find("NonGameOverUI")?.GetComponent<CanvasGroup>();
                    CanvasGroup overUI = GameObject.Find("GameOverUI")?.GetComponent<CanvasGroup>();

                    //popup.Setup(nonUI, overUI);

                    gameOverTriggered = true;
                }
            }
        }



        bool IsFogCoveringScreen()
        {
            float z = transform.position.z;

            Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Mathf.Abs(Camera.main.transform.position.z - z))) - new Vector3(margin, margin, 0);
            Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Mathf.Abs(Camera.main.transform.position.z - z))) + new Vector3(margin, margin, 0);

            bottomLeft.z = z;
            topRight.z = z;

            Bounds fogBounds = fogRenderer.bounds;

            //결과 반환, 안개가 화면을 모두 덮었다면
            return fogBounds.Contains(bottomLeft) && fogBounds.Contains(topRight);
        }
    }
}