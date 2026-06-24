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
    private readonly bool isLeftPlayer;

    [EditorReadOnly, SerializeField] private FighterState state;
    [EditorReadOnly, SerializeField] private FighterState nextState;
    public FighterState State => state;
    public void SetStanceState(StanceState newState) => state.StanceState = newState;
    public void SetMovementState(MovementState newState) => state.MovementState = newState;
    public void SetCombatState(CombatState newState) => state.CombatState = newState;


    [EditorReadOnly, SerializeField] private int Recovery;
    [EditorReadOnly, SerializeField] private int Stun;

    public bool IsAttackActive =>
        state.CombatState == CombatState.AttackActive;
    public bool IsInCombatLock =>
        state.CombatState != CombatState.Idle;
    public bool IsInMoveLock =>
        IsInCombatLock ||
        state.MovementState == MovementState.Recovery;
    public bool IsInWakeUp =>
        (state.StanceState == StanceState.KnockedDownBack || state.StanceState == StanceState.KnockedDownStomach) &&
        Stun <= GlobalAnimHashes.Wakeup.Duration;

    public Action<AttackData, AttackResult, bool> OnAttackConnected;
    public Action OnStunned;
    public Func<float, bool> OnDamageTaken;
    public Action<float> OnKnockbackTaken;

    [EditorReadOnly, SerializeField] private AnimData currentAnimData;

#if UNITY_EDITOR
    [SerializeField] private bool doDebugMode;
    [EditorReadOnly, SerializeField] private List<StateDebugInfo> stateHistory;
#endif


    public PlayerStateMachine(Transform playerRoot, bool isLeftPlayer)
    {
        anim = playerRoot.GetComponent<Animator>();
        anim.enabled = false;
        this.isLeftPlayer = isLeftPlayer;
    }
    private PlayerStateMachine() { }
    public void Dispose()
    {
        OnAttackConnected = null;
        OnStunned = null;
        OnDamageTaken = null;
        OnKnockbackTaken = null;
    }


    /// <summary>
    /// Resolve attack connection on both attacker and defender, with <paramref name="isDefender"/> as a filter flag
    /// </summary>
    public void ResolveAttack(AttackData attack, AttackResult result, bool isDefender)
    {
        OnAttackConnected?.Invoke(attack, result, isDefender);

        switch (result)
        {
            case AttackResult.KnockDown:
            {
                // Skip attacker
                if (!isDefender) break;

                OnStunned?.Invoke();
                bool playerDied = (bool)OnDamageTaken?.Invoke(attack.Damage);
                OnKnockbackTaken?.Invoke(attack.HitKb);

                (StanceState knockDownState, AnimData animData) = playerDied ?
                (StanceState.Standing, GlobalAnimHashes.Hurt.Death) : 
                attack.KnockDown switch
                {
                    AttackKnockDown.Back => (StanceState.KnockedDownBack, attack.OverrideHurtAnimData),
                    AttackKnockDown.Stomach => (StanceState.KnockedDownStomach, attack.OverrideHurtAnimData),

                    AttackKnockDown.None or _ => (default, GlobalAnimHashes.Missing),
                };
                
                SetStanceState(knockDownState);

                PlayAnimation(animData);
                Stun = attack.FrameData.HitStun;

                AudioManager.PlayKnockDownSFX();

                if (playerDied)
                {
                    SetStanceState(StanceState.KnockedDownStomach);
                    Stun = 120;
                    //    HUDManager.Instance.EndGame(isLeftPlayer);
                    //        PlayerManager.Instance.Players[0].StateMachine.Stun = 1000;
                    //        PlayerManager.Instance.Players[0].StateMachine.SetCombatState(CombatState.HitStun);
                    //        PlayerManager.Instance.Players[1].StateMachine.Stun = 1000;
                    //        PlayerManager.Instance.Players[1].StateMachine.SetCombatState(CombatState.HitStun);
                }

                break;
            }
            case AttackResult.Hit or AttackResult.CounterHit:
            {
                // Skip attacker
                if (!isDefender) break;
                
                OnStunned?.Invoke();
                bool playerDied = (bool)OnDamageTaken?.Invoke(attack.Damage);
                OnKnockbackTaken?.Invoke(attack.HitKb);

                AnimData animData = playerDied ?
                GlobalAnimHashes.Hurt.Death :
                (attack.OverrideHurtAnimData.Hash != GlobalAnimHashes.Missing.Hash ?
                attack.OverrideHurtAnimData :
                state.StanceState switch
                {
                    StanceState.Standing => attack.Level == AttackLevel.Low ?
                        GlobalAnimHashes.Hurt.Standing.Low :
                        GlobalAnimHashes.Hurt.Standing.MidHigh,

                    StanceState.Crouching => attack.Level == AttackLevel.Low ?
                        GlobalAnimHashes.Hurt.Crouching.Low :
                        GlobalAnimHashes.Hurt.Crouching.MidHigh,

                    StanceState.KnockedDownBack => GlobalAnimHashes.Hurt.KnockedDown.Back,
                    StanceState.KnockedDownStomach => GlobalAnimHashes.Hurt.KnockedDown.Stomach,

                    StanceState.Wakeup => GlobalAnimHashes.Hurt.KnockedDown.WakeUp,
                    
                    _ => GlobalAnimHashes.Missing,
                });

                PlayAnimation(animData);
                Stun = result == AttackResult.CounterHit ?
                    attack.FrameData.CounterStun :
                    attack.FrameData.HitStun;

                AudioManager.PlayHitSFX();

                if (playerDied)
                {
                    SetStanceState(StanceState.KnockedDownStomach);
                    Stun = 120;
                    //    HUDManager.Instance.EndGame(isLeftPlayer);
                    //        PlayerManager.Instance.Players[0].StateMachine.Stun = 1000;
                    //        PlayerManager.Instance.Players[0].StateMachine.SetCombatState(CombatState.HitStun);
                    //        PlayerManager.Instance.Players[1].StateMachine.Stun = 1000;
                    //        PlayerManager.Instance.Players[1].StateMachine.SetCombatState(CombatState.HitStun);
                }

                break;
            }
            case AttackResult.StandingBlocked or AttackResult.CrouchBlocked:
            {
                // Skip attacker
                if (!isDefender) break;

                OnStunned?.Invoke();
                OnKnockbackTaken?.Invoke(attack.BlockKb); 

                PlayAnimation(result == AttackResult.StandingBlocked ?
                    GlobalAnimHashes.Block.Standing :
                    GlobalAnimHashes.Block.Crouching);
                Stun = attack.FrameData.BlockStun;

                AudioManager.PlayBlockSFX();

                break;
            }
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
                result == AttackResult.CrouchBlocked;

            SetCombatState(isBlockStun ? CombatState.BlockStun : CombatState.HitStun);
        }
    }


    /// <summary>
    /// Tick down stun stat and update state based on stun recovery
    /// </summary>
    public void TickUpdateStuns()
    {
        // If player is no longer stunned this frame, recover
        if (Stun == 0 &&
            (state.CombatState == CombatState.HitStun || state.CombatState == CombatState.BlockStun))
        {
            SetCombatState(CombatState.Idle);
        }

        Recovery = Mathf.Max(Recovery - 1, 0);
        Stun = Mathf.Max(Stun - 1, 0);
    }
    /// <summary>
    /// Update animator based on current state
    /// </summary>
    public void TickUpdateAnimator()
    {
        if ((state.StanceState == StanceState.KnockedDownBack || state.StanceState == StanceState.KnockedDownStomach)
            && Stun == GlobalAnimHashes.Wakeup.Duration)
        {
            AnimData animData = state.StanceState switch
            {
                StanceState.KnockedDownBack => GlobalAnimHashes.Wakeup.Back,
                StanceState.KnockedDownStomach => GlobalAnimHashes.Wakeup.Stomach,

                _ => GlobalAnimHashes.Missing,
            };

            // Mark the players stance as no longer knocked down
            SetStanceState(StanceState.Wakeup);

            PlayAnimation(animData);
        }
        else if (!IsInMoveLock)
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

#if UNITY_EDITOR
        if (animData.Hash == GlobalAnimHashes.Missing.Hash)
        {
            DebugLogger.LogWarning($"Animator: an empty animation was requested, skipping, '{animData.Name}'");
            return;
        }
#endif

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