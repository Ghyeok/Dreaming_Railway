using System;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    private GameData _game;

    public void Init()
    {
        _game = GameDataManager.Instance.Game;
    }

    private void Start()
    {
        Init();
    }
}