using UnityEngine;

/// <summary>
/// 게임 수명주기 조정자. 진행 값은 GameData가 소유한다.
/// 값을 바꾸는 쪽은 이 클래스를 거치지 말고 GameData를 직접 호출한다
/// (여기 남은 메서드들은 데이터 변경 + 영속화/부수효과가 함께 필요한 것들뿐이다).
/// </summary>
public class GameManager : SingletonManagers<GameManager>, IManager
{
    // Time.timeScale과 한 몸이라 순수 데이터가 아니다 — 그래서 GameData로 옮기지 않는다
    public bool IsGameStopped { get; private set; }

    private GameData _data;

    public void Init()
    {
        _data = GameDataManager.Instance.Game;
        _data.SetMaxClearDay(SaveManager.Instance.LoadMaxClearDay());
    }

    public void StartDay(int day)
    {
        _data.SetCurrentDay(day);

        // 새 런이 시작되므로 이전 런의 꿈 상태(특히 IsGameOverInDream)를 지운다
        GameDataManager.Instance.Dream.Reset();
    }

    public void UpdateMaxClearDay()
    {
        if (_data.TryUpdateMaxClearDay())
            SaveManager.Instance.SaveMaxClearDay(_data.MaxClearDay);
    }

    public void StopGame()
    {
        Time.timeScale = 0f;
        IsGameStopped = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsGameStopped = false;
    }
}
