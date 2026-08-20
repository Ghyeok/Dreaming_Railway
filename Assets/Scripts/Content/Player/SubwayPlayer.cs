using System;
using UnityEngine;

/// <summary>
/// 지하철 씬의 플레이어.
/// UI 입력에 반응하는 행동(뺨 때리기 / 입석 / 스킵 / 바로 잠들기)을 수행하고
/// 그에 맞는 애니메이션과 SFX를 재생한다. 피로도에 따른 애니메이션도 여기서 갱신한다.
/// 데이터 틱 구동과 런 세션 초기화는 시스템(SubwayFlowSystem / StationSystem)의 몫이다.
/// </summary>
public class SubwayPlayer : MonoBehaviour
{
    private SubwayData _subway;
    private TirednessData _tiredness;
    private Animator _anim;

    // 입석/잠들기 포즈를 잡은 뒤에는 피로도 변화가 애니메이션을 덮어쓰지 못하게 막는다
    private bool _isPoseLocked = false;

    public event Action OnSlapSuccessed; // 뺨 성공 -> UI 쿨타임 게이지 / 횟수 갱신
    public event Action OnStood;         // 입석 -> UI 입석 버튼을 스킵 버튼으로 교체
    public event Action OnFellAsleep;    // 바로 잠들기 -> 꿈 씬 진입
    public event Action OnSkipped;       // 입석 후 스킵 -> 꿈 씬 진입

    public void Init()
    {
        _subway = GameDataManager.Instance.Subway;
        _tiredness = GameDataManager.Instance.Tiredness;
        _anim = GetComponent<Animator>();

        _isPoseLocked = false;
        _anim.SetBool("isFallAsleep", false);
        HandleTiredHalfChanged(_tiredness.IsTiredHalf); // 초기 포즈

        // Init이 두 번 호출돼도 중복 구독되지 않도록
        _tiredness.OnTiredHalfChanged -= HandleTiredHalfChanged;
        _tiredness.OnTiredHalfChanged += HandleTiredHalfChanged;
    }

    /// <summary>
    /// 뺨 때리기 — 쿨타임이 아니면 피로도를 깎고 뺨 애니메이션을 재생한다.
    /// </summary>
    public void TrySlap()
    {
        if (_subway == null || _isPoseLocked || _subway.IsSlapCoolTime) return;

        _subway.StartSlapCooldown();
        _tiredness.Decrease(_subway.TiredDecreaseBySlap);

        SoundManager.Instance.SlapSFX();
        _anim.SetTrigger("isSlap");

        OnSlapSuccessed?.Invoke();
    }

    /// <summary>
    /// 입석 — 피로도를 99.9로 고정하고 입석 포즈를 잡는다. 이후 스킵만 가능하다.
    /// </summary>
    public void TryStand()
    {
        if (_subway == null || _isPoseLocked || _subway.IsStandingCoolDown) return;

        _isPoseLocked = true;
        _tiredness.SetForced(TirednessConfigData.STANDING_TIREDNESS);

        SoundManager.Instance.StandingSFX();
        _anim.SetTrigger("isStanding");

        OnStood?.Invoke();
    }

    /// <summary>
    /// 입석 후 스킵 — 강제로 다음 노선으로 넘기고 꿈 씬으로 향한다.
    /// </summary>
    public void TrySkip()
    {
        _subway.StartStandingCooldown();
        _subway.ForceTransferByStanding();

        _anim.SetTrigger("isSkip");

        OnSkipped?.Invoke();
    }

    /// <summary>
    /// 바로 잠들기 — 잠든 포즈를 잡고 꿈 씬으로 향한다.
    /// </summary>
    public void TryFallAsleep()
    {
        if (_subway == null || _isPoseLocked) return;

        _isPoseLocked = true;
        _anim.SetBool("isFallAsleep", true);

        OnFellAsleep?.Invoke();
    }

    /// <summary>
    /// 환승 연출 애니메이션. 현재 호출부 없음.
    /// </summary>
    public void TryTransfer() => _anim.SetTrigger("isTransfer");

    private void HandleTiredHalfChanged(bool isTiredHalf)
    {
        if (_isPoseLocked) return;

        _anim.SetBool("isSleeping", isTiredHalf);
    }

    private void OnDestroy()
    {
        // 순수 C# 데이터 객체이므로 종료 중에도 null이 되지 않는다 (싱글톤 재접근 회피)
        if (_tiredness != null)
            _tiredness.OnTiredHalfChanged -= HandleTiredHalfChanged;
    }
}
