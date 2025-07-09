using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : SingletonManagers<StageSelectManager>
{
    public int currentStage;
    public int maxClearStage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override void Awake()
    {
        base.Awake();

        maxClearStage = PlayerPrefs.GetInt("MaxClearStage", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitScene()
    {

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
        if (scene.name == "StageSelect")
        {
            Debug.Log($"스테이지 씬 로드 : {scene.name}");
            UIManager.Instance.ShowSceneUI<UI_Scene>("UI_StageSelectScene");
            InitScene();
        }
    }
}
