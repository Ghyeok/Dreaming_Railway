using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_GameOverInDreamPopup : UI_Popup
{
    [SerializeField] private CanvasGroup gameOverUI;
    [SerializeField] private CanvasGroup nonGameOverUI;
    //[SerializeField] private CanvasGroup darkBackGround;
    [SerializeField] private float uiFadeDelay; // 완전히 어둠이 된 후 몇 초 뒤에 나타날지
    [SerializeField] private float uiFadeInDuration; // 몇 초에 걸쳐 나타날지
    [SerializeField] private float uiFadeOutDuration; // 몇 초에 걸쳐 사라질지


    private bool isFadingOut = false;
    private bool isFadingIn = false;
    private float timer = 0f;



    public void Setup(CanvasGroup nonUI, CanvasGroup overUI)
    {
        nonGameOverUI = nonUI;
        gameOverUI = overUI;

    }

    void start()
    {

    
    }


    void Awake()
    {
       
    }

/*

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

        darkBackGround.alpha = 0f;
        darkBackGround.blocksRaycasts = false;

    }

*/

    void Update()
    {
        if (isFadingOut) //페이드아웃 시작
        {
            timer += Time.deltaTime;
            float k = Mathf.Clamp01(timer / uiFadeInDuration);
            nonGameOverUI.alpha = 1f - k;
            nonGameOverUI.interactable = false;
            nonGameOverUI.blocksRaycasts = false;

            if (k >= 1f)
            {
                isFadingOut = false;
                isFadingIn = true;
                timer = 0f;
            }
        }


        else if (isFadingIn) //딜레이 후 페이드인 시작
        {
            timer += Time.deltaTime;

            if (timer >= uiFadeDelay)
            {    
                float t = Mathf.Clamp01(timer / uiFadeInDuration);
                gameOverUI.alpha = t;

                if (t >= 1f)
                {
                    gameOverUI.interactable = true;
                    gameOverUI.blocksRaycasts = true;
                    isFadingIn = false;

                    Debug.Log("[GameOverPopup] 연출 완료");
                }
            }
        
        }
    }

    


    private void GoToRetry()
    {//이거는 리셋한 day 지하철 씬으로 가야하는디...
        SceneManager.LoadScene("TestSubwayScene");
    }

    private void GoToMainMenu()
    {
        //메뉴창
        SceneManager.LoadScene("MainScene");
    }

    private void GoToOut()
    {
        SceneManager.LoadScene("StageSelect");
    }


}



