using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TutorialPopup : UI_Popup
{
    [Header("튜토리얼 스크립트")]
    private string[] subwayTutorialDialog =
    {
        // 지하철 씬
        "...",
        "후… 어제 좀 일찍 잘걸… ",
        "간신히 레포트 작성은 끝내고 잤지만… 오늘 수업 제대로 들을 수는 있으려나…",
        "…그보다 일단 지하철에서 졸다가 역 놓칠 것부터 걱정해야 하나…?",
        "그나저나 오늘따라 지하철에 사람이 없네… 원래도 이렇게 없었던가?",
        "뭐… 앉아서 갈 수는 있을 테니 다행이긴 하지만…",
        // 지하철에 앉은 캐릭터도 같이 표시

        "아예 안 자면서 가다간 진짜로 쓰러져버릴지도 모르니… <b>적당히 자면서 가볼까?</b>",
        "<b>내려야 할 역만 안 놓치면 되니까</b>… 흐아암",

        "음… <b>뭔가 꽉 차면 자버릴 것만 같이 생긴 막대기</b>야… 내 피로도인가…?",
        "내려야 할 정거장까지 남은 정거장 수… <b>일어나 있을 땐 언제든지 확인할수 있을거야</b>",
        "음… 근데 자버리면… <b>아마 감으로 때려맞춰야겠지…?</b>",
        "근데 진짜 너무 피곤한데… " ,
        "...내 뺨따구라도 때려서 잠을 좀 깨봐야 하나?", 

        // 팝업 내려감
        // 게임 플레이, 뺨 때리기 버튼만 활성화

        "아얏…!",
        "역시 너무 아파… <b>자주 때리지는 않고 싶은데…</b>",
        "이렇게 억지로 깨어있어봤자 <b>자는 시간만 늘어날 것 같기도 하고…</b>",

        "음… 자리에서 일어나면 <b>최소한 잠들진 않겠지…?</b>",
        "…지루한 부분은 건너뛰자구.", 
        
        // 팝업 내려감

        // 환승 후
        "계속 서 있었더니… 진짜 졸리네…",
        "<b>환승 2번 정도 할 때까진 그냥 앉아있어야지…</b> 더 서 있다간 쓰러져버릴 거ㅇㅑ…",

        "음… 뭔가 체감상 <b>시간이 훨씬 더 빠르게 흐른 느낌인데…</b>",
        "기분 탓…? 아니면…",
        " …아니겠지… 자다 보면 1시간 잔 거 같은데 3시간 지나있고 그러긴 하니까…",
        "피로도가 꽉 차지 않아도 언제든 잠들 준비는 돼 있지만…",
        "잠들면 또 구름 위를 뛰어다녀야 하려나…?",
        "일단… 이제 학교까지 무사히 가보자…",

        // 게임 클리어
        "아침부터 피곤하니 지하철 타는 것도 죽을 맛이네…",
        "수업은… 어떻게 들어야 하려나…",
        "…몇 시간 뒤의… 아니, 몇 분 후의 내가 알아서 해주겠지 뭐…",
    };
    private string[] dreamTutorialDialog =
    {
        // 꿈 속 씬
        "우음…?",
        "뒤에 뭔가 새까만 거… 계속 다가오는 게 기분 나쁜데…",
        "일단 저거랑 반대편으로 도망치는 게 맞겠지…",
        "<b>저거에 완전히 삼켜지면 진짜로 못 일어날지도 몰라…</b>",

        // 게임 플레이, 출구 발견 시
        "딱 봐도 여기로 나가는 것 같아 보이지…?",

    };
    private string[] gameoverTutorialDialog =
    {
        // 꿈 속 게임오버 시
        "아, 아직 어떻게 나가는지도 모르는데… 시야가 점점 까매져…",
        "우잉… 너무 잘 자버렸잖아…",

        // 늦게 일어나 게임오버 시
        "내려야 할 역… 놓쳐버렸네…",
    };

    private string[] subwayEmotions =
{
    "down",
    "sigh",
    "close",
    "sigh",
    "thinking",
    "smile",
    "close",
    "down",
    "mouthopen", 
    "smile",     
    "anger",     
    "close",     
    "down",    
    "slap",
    "confusion",
    "close",
    "thinking",  
    "smile",     
    "smile",
    "confusion", 

    //꿈 속에서 깬 후
    "close",    
    "anger",
    "close",
    "down",  
    "close",
    "smile",

    // 클리어
    "sigh",     
    "close",
    "smile",
};

    private string[] dreamEmotions =
    {
    "thinking",  
    "anger",     
    "close",    
    "anger",
    "smile"    
};

    private string[] gameoverEmotions =
    {
    "slap",      // (0)
    "close"      // (1)
};

    [Header("스크립트 진행도")]
    [SerializeField] private DialogState dialogState;
    [SerializeField] private int subwayIdx;
    [SerializeField] private int dreamIdx;
    [SerializeField] private int gameoverIdx;

    [Header("표정, 스크립드 참조")]
    [SerializeField] private Image playerEmotion;
    [SerializeField] private TextMeshProUGUI dialog;

    public enum Images
    {
        Player,
        Script,

    }

    public enum Texts
    {
        Dialog,

    }

    public enum DialogState
    {
        Subway,
        Dream,
        Gameover,
    }

    private void Update()
    {
        SetTutorialTrigger();

        if (Input.GetMouseButtonDown(0))
        {
            if (TutorialManager.Instance.isSlapTutorial ||
                TutorialManager.Instance.isStandingTutorial ||
                TutorialManager.Instance.isSkipTutorial)
            {
                GameManager.Instance.ResumeGame();
                TimerManager.Instance.StopTimer();
                this.gameObject.SetActive(false);
                return;
            }
            else
            {
                AdvanceDialog();
                GameManager.Instance.StopGame();
                this.gameObject.SetActive(true);
            }
        }
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        playerEmotion = GetImage((int)Images.Player);
        dialog = GetText((int)Texts.Dialog);

        dialog.text = subwayTutorialDialog[0];
        playerEmotion.sprite = ChangeEmotion("sigh");
        GameManager.Instance.StopGame();
        TutorialManager.Instance.tutorialPopup = this;
    }

    /// <param name="emotion"> anger, base, confusion, mouthopen, sigh, slap, smile, thinking 중 하나 선택</param>
    private Sprite ChangeEmotion(string emotion)
    {
        if(emotion == "none") playerEmotion.gameObject.SetActive(false);
        else playerEmotion.gameObject.SetActive(true);

        string path = "Sprites/Player/Tutorial/LD_face_";
        Sprite sprite = Resources.Load<Sprite>(path + emotion);

        return sprite;
    }

    private void ShowSubwayDialog()
    {
        IncreaseIdx();
        dialogState = DialogState.Subway;
        dialog.text = subwayTutorialDialog[subwayIdx];
        playerEmotion.sprite = ChangeEmotion(subwayEmotions[subwayIdx]);

    }

    private void ShowDreamDialog()
    {
        IncreaseIdx();
        dialogState = DialogState.Dream;
        dialog.text = dreamTutorialDialog[dreamIdx];
        playerEmotion.sprite = ChangeEmotion(dreamEmotions[dreamIdx]);
    }

    private void ShowGameOverDialog()
    {
        IncreaseIdx();
        dialogState = DialogState.Gameover;
        dialog.text = gameoverTutorialDialog[gameoverIdx];
        playerEmotion.sprite = ChangeEmotion(gameoverEmotions[gameoverIdx]);
    }

    public void AdvanceDialog()
    {
        switch (dialogState)
        {
            case DialogState.Subway:
                if (subwayIdx < subwayTutorialDialog.Length)
                {
                    ShowSubwayDialog(); // 필요시 인덱스별 감정 키로 교체
                }
                break;

            case DialogState.Dream:
                if (dreamIdx < dreamTutorialDialog.Length)
                {
                    ShowDreamDialog();
                }
                break;

            case DialogState.Gameover:
                if (gameoverIdx < gameoverTutorialDialog.Length)
                {
                    ShowGameOverDialog();
                }
                break;
        }
    }

    public void SetTutorialTrigger()
    {
        TutorialManager tm = TutorialManager.Instance;

        if(subwayIdx == tm.slapIdx)
        {
            tm.isSlapTutorial = true;
        }
        if (subwayIdx == tm.standingIdx)
        {
            tm.isStandingTutorial = true;
        }
        if (subwayIdx == tm.skipIdx)
        {
            tm.isSkipTutorial = true;
        }
    }

    public void IncreaseIdx()
    {
        if (dialogState == DialogState.Subway) subwayIdx++;
        if (dialogState == DialogState.Dream) dreamIdx++;
        if (dialogState == DialogState.Gameover) gameoverIdx++;
    }
}
