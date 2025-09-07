using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ScriptPopup : UI_Popup
{
    [Header("표정, 스크립드 참조")]
    public Image playerEmotion;
    public TextMeshProUGUI dialog;

    public enum Images
    {
        Player,
        Script
    }

    public enum Texts
    {
        Dialog,
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));

        playerEmotion = GetImage((int)Images.Player);
        dialog = GetText((int)Texts.Dialog);

        ScriptManager.Instance.scriptPopup = this;

        if (ScriptManager.Instance.isStart || ScriptManager.Instance.isClear)
        {
            ScriptManager.Instance.ShowDialog(StageSelectManager.Instance.currentStage);
            GameManager.Instance.StopGame();
            this.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ScriptManager.Instance.curIdx++;
            ScriptManager.Instance.SetScriptTrigger();

            if (ScriptManager.Instance.isStart || ScriptManager.Instance.isClear) // 팝업 진행
            {
                ScriptManager.Instance.ShowDialog(StageSelectManager.Instance.currentStage);
            }
            else
            {
                GameManager.Instance.ResumeGame();
                this.gameObject.SetActive(false);
            }
        }
    }
}
