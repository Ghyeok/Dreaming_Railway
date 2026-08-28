using UnityEngine;

public class GameManager : SingletonManagers<GameManager>, IManager
{
    private GameData _game;

    public void Init()
    {
        _game = GameDataManager.Instance.Game;
        _game.InitMaxClearDay(SaveManager.Instance.LoadMaxClearDay());
    }

    /// <summary>
    /// 게임 전체를 멈춘다. timeScale을 0으로
    /// </summary>
    public void StopGame() { Time.timeScale = 0f; }

    /// <summary>
    /// 게임을 재개한다. timeScale을 1로
    /// </summary>
    public void ResumeGame() { Time.timeScale = 1f; }

}
