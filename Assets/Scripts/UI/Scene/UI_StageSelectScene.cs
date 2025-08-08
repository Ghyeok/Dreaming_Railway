using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_StageSelectScene : UI_Scene
{
    public enum GameObjects
    {
        ButtonRoot,
    }

    public enum Buttons
    {
        Stage0,
        Stage1,
        Stage2,
        Stage3,
        Stage4,
        Stage5,
        BackButton,
    }

    public enum Images
    {
        SubwayMini,
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));   
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        GameObject stage0 = GetButton((int)Buttons.Stage0).gameObject;
        AddUIEvent(stage0, Stage0ButtonOnClicked);

        GameObject stage1 = GetButton((int)Buttons.Stage1).gameObject;
        AddUIEvent(stage1, Stage1ButtonOnClicked);

        GameObject stage2 = GetButton((int)Buttons.Stage2).gameObject;
        AddUIEvent(stage2, Stage2ButtonOnClicked);

        GameObject stage3 = GetButton((int)Buttons.Stage3).gameObject;
        AddUIEvent(stage3, Stage3ButtonOnClicked);

        GameObject stage4 = GetButton((int)Buttons.Stage4).gameObject;
        AddUIEvent(stage4, Stage4ButtonOnClicked);

        GameObject stage5 = GetButton((int)Buttons.Stage5).gameObject;
        AddUIEvent(stage5, Stage5ButtonOnClicked);

        GameObject back = GetButton((int)Buttons.BackButton).gameObject;
        AddUIEvent(back, BackButtonOnClicked);

        SetActiveTrueButtons();
    }

    private void OnEnable()
    {
        SoundManager.Instance.MainBGM();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    private void SetActiveFalseButtons()
    {
        Get<GameObject>((int)GameObjects.ButtonRoot).gameObject.SetActive(false);
    }
    
    private void SetActiveTrueButtons()
    {
        Get<GameObject>((int)GameObjects.ButtonRoot).gameObject.SetActive(true);
    }

    IEnumerator EnterToSubway()
    {
        Animator anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("ButtonClicked");
        SetActiveFalseButtons();

        StageSelectManager.Instance.InvokeStageSelect();
        yield return new WaitForSeconds(6.7f);

        SceneManager.LoadScene("TestSubwayScene");
    }

    private void BackButtonOnClicked(PointerEventData data)
    {
        SceneManager.LoadScene("MainScene");
    }

    private void Stage0ButtonOnClicked(PointerEventData data)
    {
        GameManager.Instance.gameMode = GameManager.GameMode.Tutorial;
        StageSelectManager.Instance.currentStage = 0;
        GameManager.Instance.ResetGame();

        StartCoroutine(EnterToSubway());
    }

    private void Stage1ButtonOnClicked(PointerEventData data)
    {
        StageSelectManager.Instance.currentStage = 1;
        GameManager.Instance.ResetGame();

        StartCoroutine(EnterToSubway());
    }

    private void Stage2ButtonOnClicked(PointerEventData data)
    {
        StageSelectManager.Instance.currentStage = 2;
        GameManager.Instance.ResetGame();

        StartCoroutine(EnterToSubway());
    }

    private void Stage3ButtonOnClicked(PointerEventData data)
    {
        StageSelectManager.Instance.currentStage = 3;
        GameManager.Instance.ResetGame();

        StartCoroutine(EnterToSubway());
    }

    private void Stage4ButtonOnClicked(PointerEventData data)
    {
        StageSelectManager.Instance.currentStage = 4;
        GameManager.Instance.ResetGame();

        StartCoroutine(EnterToSubway());
    }

    private void Stage5ButtonOnClicked(PointerEventData data)
    {
        StageSelectManager.Instance.currentStage = 5;
        GameManager.Instance.ResetGame();

        StartCoroutine(EnterToSubway());
    }
}
