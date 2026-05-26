using System;
using Unity.Mathematics;
using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for handling player state
/// </summary>
[System.Serializable]
public class PlayerStateMachine
{
    private readonly Animator anim;

    [EditorReadOnly, SerializeField] private FighterState state;
    [EditorReadOnly, SerializeField] private FighterState nextState;
    public FighterState State => state;
    public void SetStanceState(StanceState newState) => state.StanceState = newState;
    public void SetMovementState(MovementState newState) => state.MovementState = newState;
    public void SetCombatState(CombatState newState) => state.CombatState = newState;


    [EditorReadOnly, SerializeField] private int TimeStop;
    [EditorReadOnly, SerializeField] private int Recovery;
    [EditorReadOnly, SerializeField] private int HitStun;
    [EditorReadOnly, SerializeField] private int BlockStun;

    public bool IsTimeStopped => TimeStop > 0;
    public bool IsInCombatLock =>
        state.CombatState != CombatState.Idle;
    public bool IsInMoveLock =>
        IsInCombatLock ||
        state.MovementState == MovementState.Recovery;
    public bool IsInDashLock =>
        IsInCombatLock ||
        state.MovementState == MovementState.DashingBack ||
        state.MovementState == MovementState.DashingForward;
    public bool IsInSideStepLock =>
        IsInCombatLock ||
        state.MovementState == MovementState.SideSteppingUp ||
        state.MovementState == MovementState.SideSteppingDown;
    public bool IsStunned =>
        state.CombatState == CombatState.HitStun ||
        state.CombatState == CombatState.BlockStun;

    public Action OnStunned;
    public Action<float> OnDamageTaken;
    public Action<float> OnKnockbackTaken;

    [EditorReadOnly, SerializeField] private AnimData currentAnimData;


    public PlayerStateMachine(Transform playerRoot)
    {
        anim = playerRoot.GetComponent<Animator>();
        anim.enabled = false;
    }
    private PlayerStateMachine() { }


    /// <summary>
    /// Resolve attack connection on both attacker and defender, with <paramref name="isDefender"/> as a filter flag
    /// </summary>
    public void ResolveAttack(AttackData attack, AttackResult result, bool isDefender)
    {
        switch (result)
        {
            case AttackResult.Parried:

                TimeStop = GameRules.CombatSettings.Parry.HitStop;
                if (!isDefender)
                {
                    OnStunned?.Invoke();

                    HitStun = GameRules.CombatSettings.Parry.HitStun;
                }
                break;

            case AttackResult.Hit or AttackResult.CounterHit:

                TimeStop = attack.FrameData.HitStop;
                if (isDefender)
                {
                    OnStunned?.Invoke();
                    OnDamageTaken?.Invoke(attack.Damage);
                    OnKnockbackTaken?.Invoke(attack.HitKb);

                    bool isAttackLow = attack.Level == AttackLevel.Low;
                    AnimData animData = state.StanceState switch
                    {
                        StanceState.Standing => isAttackLow ?
                            GlobalAnimHashes.Hurt.StandingLow :
                            GlobalAnimHashes.Hurt.StandingHigh,

                        StanceState.Crouching => isAttackLow ?
                            GlobalAnimHashes.Hurt.CrouchingLow :
                            GlobalAnimHashes.Hurt.CrouchingHigh,

                        StanceState.KnockedDown => isAttackLow ?
                            GlobalAnimHashes.Hurt.KnockedDownLow :
                            GlobalAnimHashes.Hurt.KnockedDownHigh,

                        _ => GlobalAnimHashes.Missing,
                    };

                    PlayAnimation(animData);

                    HitStun = result == AttackResult.CounterHit ?
                        attack.FrameData.CounterStun :
                        attack.FrameData.HitStun;
                }
                break;

            case AttackResult.StandingBlocked or AttackResult.LowBlocked:

                TimeStop = attack.FrameData.BlockStop;
                if (isDefender)
                {
                    OnStunned?.Invoke();
                    OnKnockbackTaken?.Invoke(attack.BlockKb);

                    PlayAnimation(result == AttackResult.StandingBlocked ?
                        GlobalAnimHashes.Block.Standing :
                        GlobalAnimHashes.Block.Crouching);

                    BlockStun = attack.FrameData.BlockStun;
                }
                break;

            default:
                break;
        }

        if (!isDefender)
        {
            Recovery = attack.FrameData.Recovery;
        }

        if (HitStun > 0)
        {
            // When HitStunned, Reset BlockStun and Recovery
            BlockStun = 0;
            Recovery = 0;

            SetCombatState(CombatState.HitStun);
        }
        else if (BlockStun > 0)
        {
            SetCombatState(CombatState.BlockStun);
        }

        DebugLogger.Log(State.CombatState);
    }


    /// <summary>
    /// Update animator and tick down stun states
    /// </summary>
    public void TickUpdate()
    {
        if (TimeStop > 0)
        {
            TimeStop -= 1;
            return;
        }

        Recovery = Mathf.Clamp(Recovery - 1, 0, int.MaxValue);
        HitStun = Mathf.Clamp(HitStun - 1, 0, int.MaxValue);
        BlockStun = Mathf.Clamp(BlockStun - 1, 0, int.MaxValue);

        // If player was stunned or recovering and just recovered, set combat state to idle
        if (HitStun == 0 && BlockStun == 0 && Recovery == 0 &&
            (state.CombatState == CombatState.HitStun || state.CombatState == CombatState.BlockStun))
        {
            SetCombatState(CombatState.Idle);
        }

        if (!IsInMoveLock)
        {
            AnimData animData;

            if (state.StanceState == StanceState.Crouching)
            {
                animData = GlobalAnimHashes.Movement.Crouching;
            }
            else
            {
                animData = state.MovementState switch
                {
                    MovementState.Idle => GlobalAnimHashes.Movement.Idle,
                    MovementState.Retreating => GlobalAnimHashes.Movement.Retreat,

                    MovementState.Pushing => GlobalAnimHashes.Movement.Push,

                    _ => GlobalAnimHashes.Missing,
                };
            }

            PlayAnimation(animData);
        }

        anim.Update(GlobalGameData.TICK_TIME);
    }

    /// <summary>
    /// Play animation by <paramref name="animHash"/> blend from previous animation for <paramref name="transitionFrames"/> ticks.
    /// Skips if new animation is the same as current one.
    /// </summary>
    public void PlayAnimation(AnimData animData)
    {
        if (currentAnimData.Hash == animData.Hash) return;

        AnimData prevAnimData = currentAnimData;
        currentAnimData = animData;

        if (animData.BlendIn == 0)
        {
            anim.PlayInFixedTime(animData.Hash);
        }
        else
        {
            float blendTime = math.min(prevAnimData.BlendOut, currentAnimData.BlendIn) * GlobalGameData.TICK_TIME;
            anim.CrossFadeInFixedTime(animData.Hash, blendTime);
        }
    }

    /// <summary>
    /// Advance current animation by <paramref name="tickAdvanceCount"/> amount of ticks
    /// </summary>
    public void TickAdvanceAnimation(int tickAdvanceCount)
    {
        anim.Update(GlobalGameData.TICK_TIME * tickAdvanceCount);
    }
}