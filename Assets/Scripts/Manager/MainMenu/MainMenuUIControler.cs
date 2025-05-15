using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIControler : MonoBehaviour
{
    public void GoToStageSelect()
    {
        SceneManager.LoadScene("StageSelect"); // 바꿀 씬 이름 정확히 작성
    }
}
