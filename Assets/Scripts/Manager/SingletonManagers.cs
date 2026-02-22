using System;
using UnityEngine;

public class SingletonManagers<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance { get { Init(); return _instance; } }

    private static void Init()
    {
        if (_instance == null)
        {
            _instance = (T)FindAnyObjectByType(typeof(T));

            if (_instance == null)
            {
                CreateInstance();
            }
        }
    }

    private static void CreateInstance()
    {
        // Lazy Initialization(지연 생성)
        if (_instance == null)
        {
            GameObject go = new GameObject();
            go.name = "@" + typeof(T).Name;
            _instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
        }
    }

    public virtual void Awake()
    {
        RemoveDuplicates();
    }

    private void RemoveDuplicates()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
