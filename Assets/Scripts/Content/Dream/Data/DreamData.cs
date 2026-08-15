using UnityEngine;

/// <summary>
/// 꿈 씬 상태. 씬과 무관하게 GameDataManager가 소유한다.
/// 뺨 횟수는 여기 두지 않는다 — SubwayData.SlapNum이 원본이고, 복사본을 만들면 조용히 어긋난다.
/// </summary>
[System.Serializable]
public class DreamData
{
    [field: SerializeField] public bool IsInDream { get; private set; } = false;

    // 꿈 속에서 게임오버 되었는가 — Day가 시작될 때 Reset()으로 해제된다
    [field: SerializeField] public bool IsGameOverInDream { get; private set; } = false;

    public void EnterDream() => IsInDream = true;

    public void ExitDream() => IsInDream = false;

    public void SetGameOverInDream() => IsGameOverInDream = true;

    /// <summary>
    /// 초기값으로 초기화. 새 런(Day/무한 모드)이 시작될 때 호출한다.
    /// </summary>
    public void Reset()
    {
        IsInDream = false;
        IsGameOverInDream = false;
    }
}
