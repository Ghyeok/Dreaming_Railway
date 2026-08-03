using UnityEngine;

public static class SubwayConfigData
{
    // 시간 흐름 속도
    public const float NORMAL_FLOW_SPEED = 1f;
    public const float DREAM_MIN_FLOW_SPEED = 3.1f;
    public const float DREAM_MAX_FLOW_SPEED = 4.1f;

    // 꿈 길이 영향 
    public const float AWAKE_TIME_TIER1_MAX = 50f;
    public const float AWAKE_TIME_TIER2_MAX = 100f;
    public const float AWAKE_TIME_TIER3_MAX = 125f;

    // 게임 세팅 설정 값 (쿨타임 등등)
    public const float INITIAL_SLAP_COOP_TIME = 5f;
    public const float INITIAL_TIREDNESS_DECREASE_BY_SLAP = 3f;
    public const float INFINITE_TIREDNESS_DECREASE_BY_SLAP = 4f;
    public const int STANDING_COOLDOWN_LINES = 2;
}
