using System;

public class SubwayData
{
    public float SlapCoolTime { get; private set; } = 5f; // 뺨 때리기 쿨타임
    public int SlapNum { get; private set; } = 0; // 뺨 때린 횟수
    public float TiredDecreaseBySlap { get; private set; } = 3f; // 뺨 때리면 감소하는 피로도
    public bool IsSlapCoolTime { get; private set; } = false; // 뺨 때리기 쿨타임인지
    public float CurrentSlapCooldown { get; private set; } = 0f; // 현재 뺨 때리기 쿨타임

    public int StandingCount { get; private set; } = 0; // 입석 횟수
    public bool IsStandingCoolDown { get; private set; } = false; // 입석 쿨타임인지

    public bool IsGameOverInSubway { get; private set; } = false; // 게임 오버인지

    public static event Action OnSubwayGameOver;

    public void Reset(bool isInfiniteMode)
    {
        SlapNum = 0;
        IsSlapCoolTime = false;
        SlapCoolTime = 5f;
        CurrentSlapCooldown = 0f;
        TiredDecreaseBySlap = isInfiniteMode ? 4f : 3f;

        StandingCount = 0;
        IsStandingCoolDown = false;

        IsGameOverInSubway = false;
    }

    public void TickSlapCooldown(float deltaTime)
    {
        if (!IsSlapCoolTime) return;

        CurrentSlapCooldown -= deltaTime;
        if (CurrentSlapCooldown <= 0f)
        {
            IsSlapCoolTime = false;
            CurrentSlapCooldown = SlapCoolTime;
        }
    }

    public void StartSlapCooldown()
    {
        SlapNum++;
        IsSlapCoolTime = true;
        CurrentSlapCooldown = SlapCoolTime;
    }

    public void StartStandingCooldown()
    {
        IsStandingCoolDown = true;
        StandingCount = 0;
    }

    public void AddStandingCount()
    {
        if (!IsStandingCoolDown) return;

        StandingCount++;
        if (StandingCount >= 2)
        {
            IsStandingCoolDown = false;
            StandingCount = 0;
        }
    }

    public void SetGameOver()
    {
        if (IsGameOverInSubway) return;

        IsGameOverInSubway = true;
        OnSubwayGameOver?.Invoke();
        UIManager.Instance.ShowPopupUI<UI_Popup>("UI_GameOverPopup2");
    }
}