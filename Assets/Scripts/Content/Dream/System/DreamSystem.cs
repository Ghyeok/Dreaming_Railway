using UnityEngine;

/// <summary>
/// 꿈 도메인의 단일 변경 창구. 상태는 갖지 않고 DreamData를 구동한다.
/// 지하철/꿈 양쪽 씬 디렉터가 부르므로 DontDestroyOnLoad 싱글톤이다.
/// </summary>
public class DreamSystem : SingletonManagers<DreamSystem>, IManager
{
    private DreamData _data;

    public void Init()
    {
        _data = GameDataManager.Instance.Dream;
    }

    private void OnEnable()
    {
        FogMovement.OnDreamGameOver -= HandleGameOver;
        FogMovement.OnDreamGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        FogMovement.OnDreamGameOver -= HandleGameOver;
    }

    /// <summary>꿈 씬 진입. DreamSceneDirector가 부른다.</summary>
    public void EnterDream()
    {
        _data ??= GameDataManager.Instance.Dream;
        _data.EnterDream();
    }

    /// <summary>지하철 씬 복귀. SubwaySceneDirector가 부른다.</summary>
    public void ExitDream()
    {
        _data ??= GameDataManager.Instance.Dream;
        _data.ExitDream();
    }

    /// <summary>꿈 속 게임오버 — 안개에 덮였을 때 FogMovement가 발행한다.</summary>
    private void HandleGameOver()
    {
        _data ??= GameDataManager.Instance.Dream;
        _data.SetGameOverInDream();
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_GameOverPopup");
    }
}
