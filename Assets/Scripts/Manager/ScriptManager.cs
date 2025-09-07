using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScriptManager : SingletonManagers<ScriptManager>, IManager
{
    public string[] day1Script =
    {
        // Day 1
        "으에… 무슨 일이 있어도 일찍 자려고 했는데…",
        "어제가 숙제 제일 많이 해야 되는 날인 걸 까먹고 있었네… 흑…",
        "오늘도… 적당히 자면서… 학교까지 잘 가보자…",

        // 클리어
        "그래도… 오늘은 어제보단 덜 피곤한 것 같기도…?",
        "음… 아닌가…",
        "꿈 속에서 하도 뛰어다녀서 그렇게 느껴지는 건가…?",
    };
    public string[] day1Emotion =
    {
        // Day 1
        "confusion",
        "sigh",
        "down",

        // Day 1 클리어 감정
        "smile",
        "anger",
        "thinking",
    };

    public string[] day2Script =
    {
        // Day 2
        "오늘은… 후문 근처에서 수업이 있으니까 다른 노선으로 가야 하는데…",
        "어째서 요즘따라 지하철에 사람이 없는 것 같지…?",
        "원래도 사람이 많이 없긴 했지만… 이렇게 없었던가…",
    };
    public string[] day2Emotion =
    {
        // Day 2 감정
        "thinking",
        "anger",
        "close",
    };

    public string[] day3Script =
    {
        // Day 3
        "오늘은 예전에 예약해뒀던 테마 카페 가는 날…♪",
        "공강이기도 하고… 예약 시간대도 점심 시간대라 푹 잤으니 오늘은 안 졸아도 되겠지…",
        "음식 엄청 맛있다던데… 기대된다…",

        // 클리어
        "…?",
        "어라… 왜… 또 자버렸지…?",
        "지하철 안에서 자는 게 그새 습관이 돼 버렸나…",
    };
    public string[] day3Emotion =
    {
        // Day 3 감정
        "smile",
        "thinking",
        "smile",
        
        // Day 3 클리어 감정
        "anger",
        "close",
        "sigh",
    };

    public string[] day4Script =
    {
        // Day 4
        "과제도 일찍 끝냈고… 잠도 일찍 잤고…",
        "오늘이야말로 안 자고 풀컨디션으로 학교까지 갈 수 있겠지…?",

        // 클리어
        "…학교까지 가는 길이 원래 이렇게 멀었었나…",
        "잘못하면 지각하겠는걸…",
        "그나저나… 요 며칠 간 왜 잘 때마다 항상 똑같은 꿈을 꾼 것 같지…? 기분 탓이려나…",
    };
    public string[] day4Emotion =
    {
        // Day 4 감정
        "anger",
        "smile",
        
        // Day 4 클리어 감정
        "close",
        "slap",
        "anger",
    };

    public string[] day5Script =
    {
        // Day 5 클리어
        "흐아암…",
        "내일은 드디어 주말이니… 집 가자마자 자야겠어…",
        "음… 그나저나… 기분 탓인진 모르겠지만…",
        "지하철에 사람도 예전만큼 없고… 지하철에 타기만 해도 갑자기 졸리고…",
        "꿈도… 조금씩 다르긴 했지만 어둠에 쫓겨 구름 위를 계속해서 뛰어다닌다는 건 똑같았고…",
        "…",
        "그러고 보니…",
        "집앞 정거장도 그렇고 학교 쪽 정거장도 그렇고 내가 탔던 쪽엔 지하철 타는 곳이 없었던 것 같은데…",
    };
    public string[] day5Emotion =
    {
        // Day 5 클리어 감정
        "sigh",
        "down",
        "mouthopen",
        "anger",
        "sigh",
        "close",
        "thinking",
        "down",
    };

    public enum DialogState
    {
        Day1,
        Day2,
        Day3, 
        Day4, 
        Day5,
    }

    public UI_ScriptPopup scriptPopup;
    public int curIdx;

    public bool isStart;
    public bool isClear;

    public void Init()
    {
        ResetScript();
    }

    public void ResetScript()
    {
        curIdx = 0;
        isStart = false;
        isClear = false;
    }

    public void ShowDialog(int day)
    {
        switch (day)
        {
            case 1:
                AdvanceDialog(day1Script, day1Emotion);
                break;
            case 2:
                AdvanceDialog(day2Script, day2Emotion);
                break;
            case 3:
                AdvanceDialog(day3Script, day3Emotion);
                break;
            case 4:
                AdvanceDialog(day4Script, day4Emotion);
                break;
            case 5:
                AdvanceDialog(day5Script, day5Emotion);
                break;


        }
    }

    /// <param name="emotion"> anger, base, confusion, mouthopen, sigh, slap, smile, thinking 중 하나 선택</param>
    private Sprite ChangeEmotion(string emotion)
    {
        if (emotion == "none") scriptPopup.playerEmotion.gameObject.SetActive(false);
        else scriptPopup.playerEmotion.gameObject.SetActive(true);

        string path = "Sprites/Player/Tutorial/LD_face_";
        Sprite sprite = Resources.Load<Sprite>(path + emotion);

        return sprite;
    }

    private void AdvanceDialog(string[] script, string[] emotion)
    {
        if (curIdx < script.Length)
        {
            scriptPopup.playerEmotion.sprite = ChangeEmotion(emotion[curIdx]);
            scriptPopup.dialog.text = script[curIdx];
        }
        else
        {
            isClear = false;
            UIManager.Instance.ClosePopupUI(scriptPopup);
            GameManager.Instance.ResumeGame();
        }
    }

    public void SetScriptTrigger()
    {
        if(StageSelectManager.Instance.currentStage == 1)
        {
            if(curIdx == 3)
            {
                isStart = false;
            }
        }
        if (StageSelectManager.Instance.currentStage == 2)
        {
            if (curIdx == 3)
            {
                isStart = false;
            }
        }
        if (StageSelectManager.Instance.currentStage == 3)
        {
            if (curIdx == 3)
            {
                isStart = false;
            }
        }
        if (StageSelectManager.Instance.currentStage == 4)
        {
            if (curIdx == 2)
            {
                isStart = false;
            }
        }
        if (StageSelectManager.Instance.currentStage == 5)
        {
            if (curIdx == 0)
            {
                isStart = false;
            }
        }
    }

    public bool HasClearDialog(int day)
    {
        return day == 1 || day == 3 || day == 4 || day == 5;
    }
}
