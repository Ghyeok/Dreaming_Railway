using UnityEngine;

public class GameManager : SingletonManagers<GameManager>
{
    public enum GameState
    {
        Main,
        DaySelect,
        Subway,
        Dream,
    }

    public GameState gameState;

    public override void Awake()
    {
        base.Awake();

        UIManager.Instance.ShowSceneUI<UI_Scene>("UI_MainMenuScene");
        gameState = GameState.Main; // 메인에서 시작
    }
}
