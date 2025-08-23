using UnityEngine;

public class TutorialManager : SingletonManagers<TutorialManager> , IManager
{
    public bool isSlapTutorial;
    public bool isStandingTutorial;
    public bool isSkipTutorial;
    public bool isDreamTutorial;

    public int slapIdx = 4;
    public int standingIdx = 7;
    public int skipIdx = 8;

    public void Init()
    {

    }
}
