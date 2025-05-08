using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamManager : SingletonManagers<DreamManager>
{
    public enum PlayerState
    {
        DREAMING,
        AWAKEDREAM,
        GAMEOVER,
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterTheDream() // 지하철 -> 꿈속에 들어올 때 
    {
        SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.DEEPSLEEP;
        TiredManager.Instance.SetTiredAfterDream();
        SceneManager.LoadScene("InDream_PlayerMove");
        SoundManager.Instance.PlayAudioClip("DreamMusic", Define.Sounds.BGM);
    }
}
