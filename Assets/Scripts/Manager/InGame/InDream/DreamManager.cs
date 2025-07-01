using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamManager : SingletonManagers<DreamManager>
{
    public bool isInDream;

    public float dreamTimeSpeed;
    private float mindreamTimeSpeed;
    private float maxdreamTimeSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RandomDreamTimeSpeed()
    {
        mindreamTimeSpeed = 4f;
        maxdreamTimeSpeed = 5f;

        dreamTimeSpeed = Random.Range(mindreamTimeSpeed, maxdreamTimeSpeed);
    }

    public void SetDreamTimeSpeedNormal()
    {
        dreamTimeSpeed = 1f;
    }

    private void InitScene()
    {
        isInDream = true;
        RandomDreamTimeSpeed();
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
            UIManager.Instance.ShowSceneUI<UI_Scene>("UI_NonGameOverScene");
            InitScene();
        }
    }
}
