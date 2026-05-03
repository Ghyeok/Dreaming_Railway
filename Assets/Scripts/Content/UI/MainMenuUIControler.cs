using UnityEngine;

public class MainMenuUIControler : MonoBehaviour
{
    public void GoToStageSelect()
    {
        SceneTransitionManager.Instance.GoToStageSelect();
    }
}
