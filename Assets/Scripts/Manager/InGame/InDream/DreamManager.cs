using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamManager : SingletonManagers<DreamManager>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitScene()
    {
        SubwayPlayerManager.Instance.playerState = SubwayPlayerManager.PlayerState.DEEPSLEEP;
        TiredManager.Instance.SetTiredAfterDream();
        SoundManager.Instance.PlayAudioClip("DreamMusic", Define.Sounds.BGM);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InDream_PlayerMove")
        {
            Debug.Log($"꿈속 씬 로드 : {scene.name}");
            InitScene();
        }
    }
}
