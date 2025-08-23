using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TutorialPopup : UI_Popup
{
    [Header("튜토리얼 스크립트")]
    [SerializeField]
    private string[] subwayTutorialDialog =
    {
        // 지하철 씬
        "휴… 운 좋게 자리는 있네… 피곤하니 앉아서 가야지",
        "음… 뭔가 꽉 차면 자버릴 것만 같이 생긴 막대기야… 내 피로도인가…?",
        "내려야 할 정거장까지 남은 정거장 수… 일어나 있을 땐 언제든 알 수 있을 거야",
        "음… 근데 자버리면… 아마 감으로 때려맞춰야겠지…?",
        "너무 피곤한데… 내 뺨따구라도 때려서 잠을 좀 깨봐야 하나…?", // 팝업 내려감, idx = 4

        // 게임 플레이, 뺨 때리기 버튼만 활성화
        "아얏…! 아프다… 너무 자주 때리지는 않고 싶은데…",

        "음… 자리에서 일어나면 최소한 잠들진 않겠지…?",
        "…지루한 부분은 건너뛰자구.", // 팝업 내려감

        // 게임 플레이, 입석 버튼만 활성화
        "스킵하시겠습니까? 스킵할 경우 이번 노선에서 즉시 환승한 후 잠듭니다.",

        // 환승 후
        "우우… 다리 아파… 졸려…",
        "최소 환승 2번 할 때까진 그냥 앉아있어야지… 서 있을 기운이 업ㅅㅓ…",

        // 꿈 속에서 빠져 나옴
        "피로도가 꽉 차지 않아도 언제든 잠들 준비는 돼 있지만… 잠들면 또 그 이상한 공간으로 가겠지…?",
        "(임시)게임을 시작해볼까?",

        // 게임 클리어
        "에고, 아침부터 피곤하니 지하철 타는 것도 죽을 맛이네…",
        "오늘은 들어가면 일찍 자야지…"
    };
    [SerializeField]
    private string[] dreamTutorialDialog =
    {
        // 꿈 속 씬
        "우음…?",
        "뒤에 뭔가 새까만 거… 계속 다가오는 게 기분 나쁜데…",
        "일단 저거랑 반대편으로 도망치는 게 맞겠지…",

        // 게임 플레이, 출구 발견 시
        "딱 봐도 여기로 나가는 것 같아 보이지…?",


    };
    [SerializeField]
    private string[] gameoverTutorialDialog =
    {
        // 꿈 속 게임오버 시
        "아, 아직 어떻게 나가는지도 모르는데… 시야가 점점 까매져…",
        "우잉… 너무 잘 자버렸잖아…",

        // 늦게 일어나 게임오버 시
        "내려야 할 역… 놓쳐버렸네…",
    };

    [SerializeField]
    private string[] subwayEmotions =
{
    "sigh",      // (0)
    "mouthopen", // (1)
    "smile",     // (2)
    "anger",     // (3)
    "down",      // (4)
    "slap",      // (5)
    "thinking",  // (6)
    "smile",     // (7)
    "none",
    "confusion", // (8)
    "close",     // (9)
    "down",      // (10)
    "sigh",      // (11)
    "down"       // (12)
};

    [SerializeField]
    private string[] dreamEmotions =
    {
    "thinking",  // (0)
    "anger",     // (1)
    "close",     // (2)
    "smile"      // (3)
};

    [SerializeField]
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
            if (subwayIdx == TutorialManager.Instance.slapIdx ||
                subwayIdx == TutorialManager.Instance.standingIdx ||
                subwayIdx == TutorialManager.Instance.skipIdx)
            {
                GameManager.Instance.ResumeGame();
                this.gameObject.SetActive(false);
            }
            else
            {
                GameManager.Instance.StopGame();
                this.gameObject.SetActive(true);
                AdvanceDialog();
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
        dialogState = DialogState.Subway;
        dialog.text = subwayTutorialDialog[subwayIdx];
        playerEmotion.sprite = ChangeEmotion(subwayEmotions[subwayIdx]);

    }

    private void ShowDreamDialog()
    {
        dialogState = DialogState.Dream;
        dialog.text = dreamTutorialDialog[dreamIdx];
        playerEmotion.sprite = ChangeEmotion(dreamEmotions[dreamIdx]);
    }

    private void ShowGameOverDialog()
    {
        dialogState = DialogState.Gameover;
        dialog.text = gameoverTutorialDialog[gameoverIdx];
        playerEmotion.sprite = ChangeEmotion(gameoverEmotions[gameoverIdx]);
    }

    private void AdvanceDialog()
    {
        switch (dialogState)
        {
            case DialogState.Subway:
                if (subwayIdx < subwayTutorialDialog.Length)
                {
                    subwayIdx++;
                    ShowSubwayDialog(); // 필요시 인덱스별 감정 키로 교체
                }
                else
                {
                    // Subway 대사가 끝났다면 팝업을 닫거나 다음 상태로 전환
                    UIManager.Instance.ClosePopupUI(this);
                }
                break;

            case DialogState.Dream:
                if (dreamIdx < dreamTutorialDialog.Length)
                {
                    dreamIdx++;
                    ShowDreamDialog();
                }
                else
                {
                    UIManager.Instance.ClosePopupUI(this);
                }
                break;

            case DialogState.Gameover:
                if (gameoverIdx < gameoverTutorialDialog.Length)
                {
                    gameoverIdx++;
                    ShowGameOverDialog();
                }
                else
                {
                    UIManager.Instance.ClosePopupUI(this);
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
}
