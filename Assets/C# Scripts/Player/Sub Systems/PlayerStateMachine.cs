using System;
using System.Collections.Generic;
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
    [EditorReadOnly, SerializeField] private int Stun;

    public bool IsTimeStopped => TimeStop > 0;
    public bool IsAttackActive =>
        state.CombatState == CombatState.AttackActive;
    public bool IsInCombatLock =>
        state.CombatState != CombatState.Idle;
    public bool IsInMoveLock =>
        IsInCombatLock ||
        state.MovementState == MovementState.Recovery;

    public Action OnStunned;
    public Action<float> OnDamageTaken;
    public Action<float> OnKnockbackTaken;

    [EditorReadOnly, SerializeField] private AnimData currentAnimData;

#if UNITY_EDITOR
    [SerializeField] private bool doDebugMode;
    [EditorReadOnly, SerializeField] private List<StateDebugInfo> stateHistory;
#endif


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

                    Stun = GameRules.CombatSettings.Parry.HitStun;
                }
                break;

            case AttackResult.KnockDown:

                TimeStop = attack.FrameData.HitStop;
                if (isDefender)
                {
                    OnStunned?.Invoke();
                    OnDamageTaken?.Invoke(attack.Damage);
                    OnKnockbackTaken?.Invoke(attack.HitKb);

                    SetStanceState(StanceState.KnockedDown);

                    AnimData animData = attack.Level switch
                    {
                        AttackLevel.Low => GlobalAnimHashes.KnockDown.Low,
                        AttackLevel.Mid => GlobalAnimHashes.KnockDown.Mid,
                        AttackLevel.High => GlobalAnimHashes.KnockDown.High,

                        _ => GlobalAnimHashes.Missing,
                    };

                    PlayAnimation(animData);

                    Stun = result == AttackResult.CounterHit ?
                        attack.FrameData.CounterStun :
                        attack.FrameData.HitStun;
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
                            GlobalAnimHashes.Hurt.Standing.Low :
                            GlobalAnimHashes.Hurt.Standing.MidHigh,

                        StanceState.Crouching => isAttackLow ?
                            GlobalAnimHashes.Hurt.Crouching.Low :
                            GlobalAnimHashes.Hurt.Crouching.MidHigh,

                        _ => GlobalAnimHashes.Missing,
                    };

                    PlayAnimation(animData);

                    Stun = result == AttackResult.CounterHit ?
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

                    Stun = attack.FrameData.BlockStun;
                }
                break;

            default:
                break;
        }

        if (!isDefender)
        {
            Recovery = attack.FrameData.Recovery;
        }

        if (Stun > 0)
        {
            // When Stunned, Reset Recovery
            Recovery = 0;

            bool isBlockStun =
                result == AttackResult.StandingBlocked ||
                result == AttackResult.LowBlocked;

            SetCombatState(isBlockStun ? CombatState.BlockStun : CombatState.HitStun);
        }
    }


    /// <summary>
    /// Tick down stun stat and update state based on stun recovery
    /// </summary>
    public void TickUpdateStuns()
    {
        if (TimeStop > 0)
        {
            TimeStop -= 1;
            return;
        }

        // If player is no longer stunned this frame, recover
        if (Stun == 0 &&
            (state.CombatState == CombatState.HitStun || state.CombatState == CombatState.BlockStun))
        {
            SetCombatState(CombatState.Idle);
        }

        Recovery = Mathf.Clamp(Recovery - 1, 0, int.MaxValue);
        Stun = Mathf.Clamp(Stun - 1, 0, int.MaxValue);
    }
    /// <summary>
    /// Update animator based on current state
    /// </summary>
    public void TickUpdateAnimator()
    {
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

#if UNITY_EDITOR
        if (doDebugMode)
        {
            stateHistory.Add(new StateDebugInfo(state, Stun, Recovery));
        }
#endif
    }

    /// <summary>
    /// Play animation by <paramref name="animHash"/> blend from previous animation for <paramref name="transitionFrames"/> ticks.
    /// Skips if new animation is the same as current one.
    /// </summary>
    public void PlayAnimation(AnimData animData)
    {
        if (!animData.AllowSelfInterrupt && currentAnimData.Hash == animData.Hash) return;

        if (animData.Hash == GlobalAnimHashes.Missing.Hash)
        {
            DebugLogger.LogWarning("Animator: an empty animation was requested, skipping");
            return;
        }

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


[System.Serializable]
public struct StateDebugInfo
{
    [EditorReadOnly, SerializeField] private StanceState stanceState;
    [EditorReadOnly, SerializeField] private MovementState movementState;
    [EditorReadOnly, SerializeField] CombatState combatState;

    [EditorReadOnly, SerializeField] private int stun;
    [EditorReadOnly, SerializeField] private int recovery;


    public StateDebugInfo(FighterState state, int stunLeft, int recoveryLeft)
    {
        stanceState = state.StanceState;
        movementState = state.MovementState;
        combatState = state.CombatState;

        stun = stunLeft;
        recovery = recoveryLeft;
    }
}