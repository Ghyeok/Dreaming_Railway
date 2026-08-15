using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_StageSelectScene : UI_Scene
{
    private const string KEY_SUBWAY_POS_X = "SubwayPosX";

    private SubwayMiniMove subwayMiniMove;
    private Animator _animator;

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

        AddUIEvent(GetButton((int)Buttons.Stage0).gameObject, data => OnStageClicked(0, subwayMiniMove.position0));
        AddUIEvent(GetButton((int)Buttons.Stage1).gameObject, data => OnStageClicked(1, subwayMiniMove.position1));
        AddUIEvent(GetButton((int)Buttons.Stage2).gameObject, data => OnStageClicked(2, subwayMiniMove.position2));
        AddUIEvent(GetButton((int)Buttons.Stage3).gameObject, data => OnStageClicked(3, subwayMiniMove.position3));
        AddUIEvent(GetButton((int)Buttons.Stage4).gameObject, data => OnStageClicked(4, subwayMiniMove.position4));
        AddUIEvent(GetButton((int)Buttons.Stage5).gameObject, data => OnStageClicked(5, subwayMiniMove.position5));
        AddUIEvent(GetButton((int)Buttons.BackButton).gameObject, BackButtonOnClicked);

        ShowAllButtons();
    }

    void Start()
    {
        Init();
        _animator = GetComponentInChildren<Animator>();

        LoadSubwayPosition();
        LoadStageLock();
    }

    private void HideAllButtons()
    {
        Get<GameObject>((int)GameObjects.ButtonRoot).gameObject.SetActive(false);
    }

    private void ShowAllButtons()
    {
        Get<GameObject>((int)GameObjects.ButtonRoot).gameObject.SetActive(true);
    }

    private void OnStageClicked(int day, RectTransform position)
    {
        if (day == 0)
        {
            GameDataManager.Instance.Game.SetGameMode(GameMode.Tutorial);
        }
        else
        {
            GameDataManager.Instance.Game.SetGameMode(GameMode.NormalMode);
            ScriptManager.Instance.isStart = true;
        }
        GameManager.Instance.StartDay(day);
        StartCoroutine(EnterToSubway(position));
    }

    private IEnumerator EnterToSubway(RectTransform targetPosition)
    {
        float defaultX = subwayMiniMove.position0.anchoredPosition.x;

        float priorPosX = PlayerPrefs.HasKey(KEY_SUBWAY_POS_X)
            ? PlayerPrefs.GetFloat(KEY_SUBWAY_POS_X)
            : defaultX;

        if (Mathf.Abs(priorPosX - targetPosition.anchoredPosition.x) > 10f)
        {
            subwayMiniMove.MoveToPosition(targetPosition);
            yield return new WaitForSeconds(1.5f); // 스테이지 선택 후 대기 시간
        }

        SaveSubwayPosition(targetPosition.anchoredPosition);

        _animator.SetTrigger("ButtonClicked");
        HideAllButtons();

        yield return new WaitForSeconds(7f); // 애니메이션 재생 대기 시간

        SceneTransitionManager.Instance.GoToSubway();
    }

    private void BackButtonOnClicked(PointerEventData data)
    {
        SceneTransitionManager.Instance.GoToMain();
    }

    private void SaveSubwayPosition(Vector2 position)
    {
        PlayerPrefs.SetFloat(KEY_SUBWAY_POS_X, position.x);
        PlayerPrefs.Save();
    }

    private void LoadSubwayPosition()
    {
        float defaultX = subwayMiniMove.position0.anchoredPosition.x;
        float x = PlayerPrefs.GetFloat(KEY_SUBWAY_POS_X, defaultX);
        subwayMiniMove.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 464f);
    }

    private void LoadStageLock()
    {
        int m = GameDataManager.Instance.Game.MaxClearDay;

        for (int i = 0; i <= 5; i++)
            SetStageButtonState(i, i <= m + 1);

        for (int i = 0; i <= 4; i++)
            GetImage((int)Images.UnderBar + i).color = i <= m ? Color.white : new Color(0.65f, 0.65f, 0.65f);
    }

    private void SetStageButtonState(int stageIndex, bool unlocked)
    {
        var btn = GetButton((int)Buttons.Stage0 + stageIndex);
        var img = btn.GetComponent<Image>();
        var cg = Util.GetOrAddComponent<CanvasGroup>(btn.gameObject);

        img.sprite = unlocked ? stageUnlock : stageLock;
        img.raycastTarget = unlocked;
        btn.interactable = unlocked;
        cg.blocksRaycasts = unlocked;
        cg.interactable = unlocked;
    }
}