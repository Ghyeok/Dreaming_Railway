/// <summary>
/// 튜토리얼 진행 인덱스 상수.
/// TutorialManager는 씬에 배치되지 않고 런타임에 자동 생성되므로,
/// 인스펙터 오버라이드가 존재할 수 없어 const로 고정해도 동작이 같다.
/// </summary>
public static class TutorialConfigData
{
    // 지하철 튜토리얼 — subwayIdx가 이 값에 도달하면 해당 단계가 트리거된다
    public const int SLAP_IDX = 12;
    public const int STANDING_IDX = 16;
    public const int SKIP_IDX = 17;
    public const int ENTER_DREAM_IDX = 19;
    public const int SUBWAY_END_IDX = 25;
    public const int GAME_CLEAR_IDX = 28;
    // UI 힌트 노출 시점 — 기존 UI_TutorialPopup.Update()의 매직넘버 8 / 9
    public const int TIREDNESS_HINT_IDX = 8;
    public const int TRANSFER_HINT_IDX = 9;

    // 꿈 튜토리얼 — dreamIdx 기준
    public const int MOVE_IDX = 3;
    public const int EXIT_IDX = 4;

    // 게임오버 튜토리얼 분기 — gameoverIdx의 시작값
    public const int DARK_GAMEOVER_IDX = 0;  // 꿈속에서 게임오버
    public const int PASS_GAMEOVER_IDX = 1;  // 환승 실패로 게임오버
}
