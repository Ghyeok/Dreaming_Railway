using UnityEngine;

public class TutorialManager : SingletonManagers<TutorialManager> , IManager
{
    public bool isSlapTutorial;
    public bool isStandingTutorial;
    public bool isSkipTutorial;
    public bool isDreamTutorial;

    public int slapIdx = 12;
    public int standingIdx = 17;
    public int skipIdx = 18;

    public UI_TutorialPopup tutorialPopup;

    public void Init()
    {
        ResetTutorial();
    }

    public void ResetTutorial()
    {
        isSlapTutorial = false;
        isStandingTutorial = false;
        isSkipTutorial = false;
        isDreamTutorial = false;
    }
}
