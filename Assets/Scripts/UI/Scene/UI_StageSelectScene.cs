using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_StageSelectScene : UI_Scene
{
    private SubwayMiniMove subwayMiniMove;

    [SerializeField] private Sprite stageLock;
    [SerializeField] private Sprite stageUnlock;

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
        UnderBar,
        UnderBar1,
        UnderBar2,
        UnderBar3,
        UnderBar4,
        SubwayMini,
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        subwayMiniMove = GetImage((int)Images.SubwayMini).GetComponent<SubwayMiniMove>();

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
        UI_FadeBlackPanel fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeBlackPanel>();
        fadePanel.StartFadeIn(0.3f);

        LoadSubwayPosition(); //저장된 게임 정보
        LoadStageLock();
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

    IEnumerator EnterToSubway(RectTransform newPositionTransform)
    {
        Vector2 newPosition = newPositionTransform.anchoredPosition; //벡터 형태로 수정

        float priorPosX = PlayerPrefs.HasKey("SubwayPosX") ? PlayerPrefs.GetFloat("SubwayPosX") : subwayMiniMove.position0.anchoredPosition.x;

        if (Mathf.Abs(priorPosX - newPosition.x) > 10f) //저장된 위치와 새로 입력된 위치가 다르다면
        {
            subwayMiniMove.MoveToPosition(newPositionTransform);

            yield return new WaitForSeconds(3f); // 미니 버스 움직이는 시간
        }

        SaveSubwayPosition(newPosition);

        Animator anim = GetComponentInChildren<Animator>();
        anim.SetTrigger("ButtonClicked");
        SetActiveFalseButtons();

        StageSelectManager.Instance.InvokeStageSelect();
        yield return new WaitForSeconds(6.4f); //애니매이션 연출 시간

        SceneManager.LoadScene("TestSubwayScene");
    }

    private void BackButtonOnClicked(PointerEventData data)
    {
        StartCoroutine(FadeAndLoadScene("MainScene"));
    }

    private void Stage0ButtonOnClicked(PointerEventData data)
    {
        GameManager.Instance.gameMode = GameManager.GameMode.Tutorial;
        StageSelectManager.Instance.currentStage = 0;
        GameManager.Instance.ResetGame();

        StartCoroutine(EnterToSubway(subwayMiniMove.position0)); //위치 정보 전달
    }

    private void Stage1ButtonOnClicked(PointerEventData data)
    {
        GameManager.Instance.gameMode = GameManager.GameMode.Normal;
        StageSelectManager.Instance.currentStage = 1;
        GameManager.Instance.ResetGame();

        ScriptManager.Instance.isStart = true;
        StartCoroutine(EnterToSubway(subwayMiniMove.position1)); //위치 정보 전달
    }

    private void Stage2ButtonOnClicked(PointerEventData data)
    {
        GameManager.Instance.gameMode = GameManager.GameMode.Normal;
        StageSelectManager.Instance.currentStage = 2;
        GameManager.Instance.ResetGame();

        ScriptManager.Instance.isStart = true;
        StartCoroutine(EnterToSubway(subwayMiniMove.position2)); //위치 정보 전달
    }

    private void Stage3ButtonOnClicked(PointerEventData data)
    {
        GameManager.Instance.gameMode = GameManager.GameMode.Normal;
        StageSelectManager.Instance.currentStage = 3;
        GameManager.Instance.ResetGame();

        ScriptManager.Instance.isStart = true;
        StartCoroutine(EnterToSubway(subwayMiniMove.position3)); //위치 정보 전달
    }

    private void Stage4ButtonOnClicked(PointerEventData data)
    {
        GameManager.Instance.gameMode = GameManager.GameMode.Normal;
        StageSelectManager.Instance.currentStage = 4;
        GameManager.Instance.ResetGame();

        ScriptManager.Instance.isStart = true;
        StartCoroutine(EnterToSubway(subwayMiniMove.position4)); //위치 정보 전달
    }

    private void Stage5ButtonOnClicked(PointerEventData data)
    {
        GameManager.Instance.gameMode = GameManager.GameMode.Normal;
        StageSelectManager.Instance.currentStage = 5;
        GameManager.Instance.ResetGame();

        ScriptManager.Instance.isStart = true;
        StartCoroutine(EnterToSubway(subwayMiniMove.position5)); //위치 정보 전달
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        UI_FadeBlackPanel fadePanel = UIManager.Instance.ShowPopupUI<UI_FadeBlackPanel>();

        fadePanel.Init();
        yield return fadePanel.Fade(0f, 1f, 0.3f); //페이드아웃

        // 비동기 씬 로드
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false; // 페이드아웃 끝난 뒤 활성화

        //씬이 거의 다 로드될 때까지 대기
        while (async.progress < 0.9f)
        {
            yield return null;
        }
        async.allowSceneActivation = true;


        yield return null;
    }

    private void SaveSubwayPosition(Vector2 position)
    {
        PlayerPrefs.SetFloat("SubwayPosX", position.x); //x값만 저장
        PlayerPrefs.Save();
    }

    private void LoadSubwayPosition()
    {
        // 저장된 위치 정보가 있는지 확인
        if (PlayerPrefs.HasKey("SubwayPosX"))
        {
            float x = PlayerPrefs.GetFloat("SubwayPosX");
            Vector2 savedPosition = new Vector2(x, 464f);

            subwayMiniMove.transform.GetComponent<RectTransform>().anchoredPosition = savedPosition;
        }
        else
        {
            subwayMiniMove.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(subwayMiniMove.position0.anchoredPosition.x, 464f);
        }
    }

    private void LoadStageLock()
    {
        // 마지막으로 클리어한 스테이지의 인덱스 (아무 것도 안 깼으면 -1)
        int m = PlayerPrefs.GetInt("MaxClearStage", -1);

        // 1) m+1..5 잠금
        for (int i = m + 1; i <= 5; i++)
        {
            var btn = GetButton((int)Buttons.Stage0 + i);
            var img = btn.GetComponent<Image>();
            var b = btn.GetComponent<Button>();
            var cg = Util.GetOrAddComponent<CanvasGroup>(btn.gameObject);

            img.sprite = stageLock;
            img.raycastTarget = false;
            b.interactable = false;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        // 언더바 잠금 색상: m+1..4
        for (int i = m + 1; i <= 4; i++)
        {
            GetImage((int)Images.UnderBar + i).GetComponent<Image>().color = new Color(0.65f, 0.65f, 0.65f);
        }

        // 2) 0..m 잠금해제
        for (int i = 0; i <= m + 1; i++)
        {
            if (i == 5) return;

            var btn = GetButton((int)Buttons.Stage0 + i);
            var img = btn.GetComponent<Image>();
            var b = btn.GetComponent<Button>();
            var cg = Util.GetOrAddComponent<CanvasGroup>(btn.gameObject);

            img.sprite = stageUnlock;
            img.raycastTarget = true;
            b.interactable = true;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        // 언더바 해제 색상: 0..m (언더바는 0..4까지만 존재하니 한 번 더 가드)
        for (int i = 0; i <= m + 1; i++)
        {
            if (i > 4) return;

            GetImage((int)Images.UnderBar + i).GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
    }
}