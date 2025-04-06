using System;
using UnityEngine;

public class UIManager : Managers<UIManager>
{
    private GameObject _root;

    public static GameObject Root
    {
        get
        {
            if (Instance._root == null)
            {
                GameObject root = GameObject.Find("UI_Root");
                if (root == null)
                {
                    root = new GameObject { name = "UI_Root" };
                }

                Instance._root = root;
                return Instance._root;
            }
            else
            {
                return Instance._root;
            }
        }
    }

    // 나중에 ExitButton 있는 UI 클래스에서 호출하세요
    // UIManager.Instance.OnExitButton(); 이런식으로
    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // UI 코드가 한곳에 집중됩니다. 유지보수 편리하고 재사용성도 좋습니다
    // UIManager.LoadUI(Define.UIType.PauseUI) 이런식으로 쓰시면 됩니다
    public static GameObject LoadUI(Define.UIType newUIType)
    {
        GameObject ui;
        try
        {
            ui = Instantiate(Resources.Load<GameObject>($"Prefabs/UIs/{newUIType}"));
            ui.transform.parent = Root.transform;
            return ui;
        }
        catch
        {
            Debug.LogError($"[Error] UI Load 실패 : Prefabs/UIs/{newUIType} 확인 바람");
            return null;
        }
    }
    
    // 이건 String으로 호출하는거
    public static GameObject LoadUI(string newUIType)
    {
        GameObject ui;
        try
        {
            ui = Instantiate(Resources.Load<GameObject>($"Prefabs/UIs/{newUIType}"));
            ui.transform.parent = Root.transform;
            return ui;
        }
        catch
        {
            Debug.LogError($"[Error] UI Load 실패 : Prefabs/UIs/{newUIType} 확인 바람");
            return null;
        }
    }
}
