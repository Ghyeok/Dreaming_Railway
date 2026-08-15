using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScriptManager : SingletonManagers<ScriptManager>, IManager
{
    public struct DialogLine
    {
        public string Text { get; }
        public string Emotion { get; }

        public DialogLine(string text, string emotion)
        {
            Text = text;
            Emotion = emotion;
        }
    }

    private readonly DialogLine[] day1Dialog =
    {
        new DialogLine("으에… 무슨 일이 있어도 일찍 자려고 했는데…", "confusion"),
        new DialogLine("어제가 숙제 제일 많이 해야 되는 날인 걸 까먹고 있었네… 흑…", "sigh"),
        new DialogLine("오늘도… 적당히 자면서… 학교까지 잘 가보자…", "down"),
        new DialogLine("그래도… 오늘은 어제보단 덜 피곤한 것 같기도…?", "smile"),
        new DialogLine("음… 아닌가…", "anger"),
        new DialogLine("꿈 속에서 하도 뛰어다녀서 그렇게 느껴지는 건가…?", "thinking"),
    };

    private readonly DialogLine[] day2Dialog =
    {
        new DialogLine("오늘은… 후문 근처에서 수업이 있으니까 다른 노선으로 가야 하는데…", "thinking"),
        new DialogLine("어째서 요즘따라 지하철에 사람이 없는 것 같지…?", "anger"),
        new DialogLine("원래도 사람이 많이 없긴 했지만… 이렇게 없었던가…", "close"),
    };

    private readonly DialogLine[] day3Dialog =
    {
        new DialogLine("오늘은 예전에 예약해뒀던 테마 카페 가는 날…♪", "smile"),
        new DialogLine("공강이기도 하고… 예약 시간대도 점심 시간대라 푹 잤으니 오늘은 안 졸아도 되겠지…", "thinking"),
        new DialogLine("음식 엄청 맛있다던데… 기대된다…", "smile"),
        new DialogLine("…?", "anger"),
        new DialogLine("어라… 왜… 또 자버렸지…?", "close"),
        new DialogLine("지하철 안에서 자는 게 그새 습관이 돼 버렸나…", "sigh"),
    };

    private readonly DialogLine[] day4Dialog =
    {
        new DialogLine("과제도 일찍 끝냈고… 잠도 일찍 잤고…", "anger"),
        new DialogLine("오늘이야말로 안 자고 풀컨디션으로 학교까지 갈 수 있겠지…?", "smile"),
        new DialogLine("…학교까지 가는 길이 원래 이렇게 멀었었나…", "close"),
        new DialogLine("잘못하면 지각하겠는걸…", "slap"),
        new DialogLine("그나저나… 요 며칠 간 왜 잘 때마다 항상 똑같은 꿈을 꾼 것 같지…? 기분 탓이려나…", "anger"),
    };

    private readonly DialogLine[] day5Dialog =
    {
        new DialogLine("흐아암…", "sigh"),
        new DialogLine("내일은 드디어 주말이니… 집 가자마자 자야겠어…", "down"),
        new DialogLine("음… 그나저나… 기분 탓인진 모르겠지만…", "mouthopen"),
        new DialogLine("지하철에 사람도 예전만큼 없고… 지하철에 타기만 해도 갑자기 졸리고…", "anger"),
        new DialogLine("꿈도… 조금씩 다르긴 했지만 어둠에 쫓겨 구름 위를 계속해서 뛰어다닌다는 건 똑같았고…", "sigh"),
        new DialogLine("…", "close"),
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
                AdvanceDialog(day1Dialog);
                break;
            case 2:
                AdvanceDialog(day2Dialog);
                break;
            case 3:
                AdvanceDialog(day3Dialog);
                break;
            case 4:
                AdvanceDialog(day4Dialog);
                break;
            case 5:
                AdvanceDialog(day5Dialog);
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

    private void AdvanceDialog(DialogLine[] dialog)
    {
        if (curIdx < dialog.Length)
        {
            scriptPopup.playerEmotion.sprite = ChangeEmotion(dialog[curIdx].Emotion);
            scriptPopup.dialog.text = dialog[curIdx].Text;
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
        if(GameDataManager.Instance.Game.CurrentDay == 1)
        {
            if(curIdx == 3)
            {
                isStart = false;
            }
        }
        if (GameDataManager.Instance.Game.CurrentDay == 2)
        {
            if (curIdx == 3)
            {
                isStart = false;
            }
        }
        if (GameDataManager.Instance.Game.CurrentDay == 3)
        {
            if (curIdx == 3)
            {
                isStart = false;
            }
        }
        if (GameDataManager.Instance.Game.CurrentDay == 4)
        {
            if (curIdx == 2)
            {
                isStart = false;
            }
        }
        if (GameDataManager.Instance.Game.CurrentDay == 5)
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
