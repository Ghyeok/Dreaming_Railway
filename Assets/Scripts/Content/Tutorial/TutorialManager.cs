using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TutorialManager;
using static UI_TutorialPopup;

public class TutorialManager : SingletonManagers<TutorialManager>, IManager
{
    public enum DialogState
    {
        Subway,
        Dream,
        Gameover,
    }

    public DialogState dialogState;
    public bool startFlowTime;
    public bool startIncreaseTired;

    [Header("스크립트 진행도")]
    public int subwayIdx;
    public int dreamIdx;
    public int gameoverIdx;

    [Header("지하철 & 게임 클리어 튜토리얼")]
    public bool isSubwayTutorial;
    public bool isSlapTutorial;
    public bool isStandingTutorial;
    public bool isSkipTutorial;
    public bool isEnterDreamTutorial;
    public bool isSubwayTutorialEnd;
    public bool isGameClearTutorial;

    [Header("꿈 속 튜토리얼")]
    public bool isDreamTutorial;
    public bool isMoveTutorial;
    public bool isExitTutorial;

    [Header("게임 오버 튜토리얼")]
    public bool isGameoverTutorial;
    public bool isDarkGameOverTutorial;
    public bool isPassedGameOverTutorial;

    [Header("팝업이 내려가기 전까지의 인덱스")]
    public int slapIdx = 12; 
    public int standingIdx = 16;
    public int skipIdx = 17;
    public int enterDreamIdx = 19;
    public int subwayEndIdx = 25;
    public int gameClearIdx = 28;

    public int moveIdx = 3;
    public int exitIdx = 4;

    public int darkIdx = 0;
    public int passIdx = 1;

    public UI_TutorialPopup tutorialPopup;

    public void Init()
    {
        ResetTutorial();
    }

    public void ResetTutorial()
    {
        dialogState = DialogState.Subway;
        startFlowTime = false;
        startIncreaseTired = false;

        isSubwayTutorial = true;
        isSlapTutorial = false;
        isStandingTutorial = false;
        isSkipTutorial = false;
        isSubwayTutorialEnd = false;
        isGameClearTutorial = false;
       
        isDreamTutorial = false;
        isMoveTutorial = false;
        isExitTutorial = false;

        isGameoverTutorial = false;
        isDarkGameOverTutorial = false;
        isPassedGameOverTutorial = false;

        subwayIdx = 0;
        dreamIdx = 0;
        gameoverIdx = 0;
    }

    public void IncreaseIdx()
    {
        if (TutorialManager.Instance.dialogState == TutorialManager.DialogState.Subway) subwayIdx++;
        if (TutorialManager.Instance.dialogState == TutorialManager.DialogState.Dream) dreamIdx++;
        if (TutorialManager.Instance.dialogState == TutorialManager.DialogState.Gameover) gameoverIdx++;
    }

    public void SetTutorialTrigger()
    {
        if (subwayIdx == slapIdx) isSlapTutorial = true;
        else isSlapTutorial = false;

        if (subwayIdx == standingIdx) isStandingTutorial = true;
        else isStandingTutorial = false;

        if (subwayIdx == skipIdx) isSkipTutorial = true;
        else isSkipTutorial = false;

        if (subwayIdx == enterDreamIdx) isEnterDreamTutorial = true;
        else isEnterDreamTutorial = false;

        if (subwayIdx == enterDreamIdx)
        {
            startFlowTime = true;
            startIncreaseTired = true;
        }

        if (subwayIdx == subwayEndIdx) isSubwayTutorialEnd = true;
        else isSubwayTutorialEnd = false;

        if (subwayIdx == gameClearIdx) isGameClearTutorial = true;
        else isGameClearTutorial = false;

        if (dreamIdx == moveIdx) isMoveTutorial = true;
        else isMoveTutorial = false;

        if (dreamIdx == exitIdx) isExitTutorial = true;
        else isExitTutorial= false;
    }

    private void StopIncreaseTired()
    {
        if(dialogState == DialogState.Gameover)
        {
            startIncreaseTired = false;
        }
    }

    private IEnumerator LoadTutorialOverlay()
    {
        // 튜토리얼 전용 씬을 추가로 올림
        var op = SceneManager.LoadSceneAsync(SceneName.Tutorial, LoadSceneMode.Additive);
        yield return op;

        //if (!TutorialManager.Instance.isSubwayTutorialEnd || TutorialManager.Instance.isPassedGameOverTutorial || IsGameOverInSubway)
        //{
        //    // 튜토리얼 팝업 띄우기
        //    UIManager.Instance.ShowPopupUI<UI_Popup>("UI_TutorialPopup");
        //}

        //TutorialManager.Instance.dialogState = TutorialManager.DialogState.Subway;
        //TutorialManager.Instance.isSubwayTutorial = true;
        //TutorialManager.Instance.isDreamTutorial = false;
    }

    private IEnumerator LoadScriptOverlay()
    {
        var op = SceneManager.LoadSceneAsync(SceneName.Script, LoadSceneMode.Additive);
        yield return op;

        ScriptManager.Instance.SetScriptTrigger();
        // 스크립트 팝업이 뜨는 경우는 처음 시작했을 때와 게임을 클리어 했을 때
        if (ScriptManager.Instance.isStart)
        {
            UIManager.Instance.ShowPopupUI<UI_ScriptPopup>("UI_ScriptPopup");
        }
    }
}
