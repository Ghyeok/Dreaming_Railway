using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : SingletonManagers<StageSelectManager>, IManager
{
    public int currentStage;
    public int maxClearStage;

    public static event Action StageSelected;

    public void Init()
    {
        maxClearStage = PlayerPrefs.GetInt("MaxClearStage", 0);
    }

    public void InvokeStageSelect()
    {
        StageSelected?.Invoke();
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
