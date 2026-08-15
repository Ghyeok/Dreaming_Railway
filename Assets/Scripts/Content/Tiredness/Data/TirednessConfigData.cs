using UnityEngine;

public static class TirednessConfigData
{
    public const float MAX_TIREDNESS = 100f;
    public const float INITIAL_TIREDNESS = 30f;
    public const float DREAM_RECOVERY_TIME_THRESHOLD = 100f; // 꿈 진입 시 피로도 조정 기준 시간 (초)
    public const float STANDING_TIREDNESS = 99.9f; // 입석 시 고정되는 피로도 (100 미만이라 자동 꿈 진입이 발생하지 않음)
}
