using UnityEngine;
using UnityEngine.SceneManagement;


public class UI_GameOverPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup gameOverUI;
    [SerializeField] private CanvasGroup nonGameOverUI;
    [SerializeField] private GameObject DarkBackGround;

    [SerializeField] private float uiFadeDelay; // 게임오버 후 몇 초 뒤에 나타날지  
    [SerializeField] private float uiFadeInDuration; // 몇 초에 걸쳐 나타날지
    [SerializeField] private float uiFadeOutDuration; // 몇 초에 걸쳐 사라질지


    private bool isGameOverUIFading = false;
    private float uiFadeInTimer; //게임오버 후, 팝업 생성 전 타이머
    private float uiFadeOutTimer;
    private bool fadeInStarted = false;
    


    void Start()
    {

    }

    void Awake()
    {
       
    }

    public void TriggerGameOver()
    {
        Debug.Log("게임오버 페이딩 시작");
        isGameOverUIFading = true;//게임오버 팝업 생성될 것
        //fadeOutStarted = true;//기존 움직임 버튼 페이드 아웃

        uiFadeInTimer = 0f;
        uiFadeOutTimer = 0f;

        gameOverUI.alpha = 0f;
        gameOverUI.interactable = false;
        gameOverUI.blocksRaycasts = false;

    }

    void Update()
    {
        if (!isGameOverUIFading) return; //페이딩 끝

        uiFadeInTimer += Time.deltaTime;
        uiFadeOutTimer += Time.deltaTime;

        //기존 ui 페이드 아웃 
        if (FogMovement.fadeOutStarted)
        {
            float k = Mathf.Clamp01(uiFadeOutTimer / uiFadeOutDuration);
            nonGameOverUI.alpha = 1 - k;
            nonGameOverUI.interactable = false;

            if (k >= 1f)
            {
                FogMovement.fadeOutStarted = false;
            }

        }



        if (!fadeInStarted && uiFadeInTimer >= uiFadeDelay)
        {//딜레이 후 페이드인 시작
            fadeInStarted = true;
            uiFadeInTimer = 0f; //타이머 초기화
        }

        // 페이드 연출
        if (fadeInStarted)
        {
            float t = Mathf.Clamp01(uiFadeInTimer / uiFadeInDuration);
            gameOverUI.alpha = t;

            if (t >= 1f)
            {
                gameOverUI.interactable = true;
                gameOverUI.blocksRaycasts = true;
                isGameOverUIFading = false; // 연출 종료
            }
        }
    }
    


    public void GoToRetry()
    {
        SceneManager.LoadScene("TestSubwayScene");
    }

    public void GoToMainMenu()
    {
        //메뉴창
        DarkBackGround.SetActive(true);
    }

    public void GoToOut()
    {
        SceneManager.LoadScene("StageSelect");
    }


}

