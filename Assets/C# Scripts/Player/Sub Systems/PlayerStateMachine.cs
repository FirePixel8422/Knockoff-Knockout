using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for handling player state
/// </summary>
[System.Serializable]
public class PlayerStateMachine
{
    [SerializeField] private FighterState state;
    public FighterState State => state;
    public void SetStanceState(StanceState newState) => state.StanceState = newState;
    public void SetMovementState(MovementState newState) => state.MovementState = newState;
    public void SetCombatState(CombatState newState) => state.CombatState = newState;


    [EditorReadOnly] public int TimeStop;
    [EditorReadOnly] public int Recovery;
    [EditorReadOnly] public int HitStun;
    [EditorReadOnly] public int BlockStun;

    public bool IsInActionLock =>
        State.CombatState != CombatState.Idle ||
        State.MovementState == MovementState.Dashing ||
        State.MovementState == MovementState.SideStepping;

    private readonly Animator anim;

    private readonly int crouchingAnimationHash = Animator.StringToHash("Crouching");
    private readonly int idleAnimationHash = Animator.StringToHash("Idle");
    private readonly int retreatAnimationHash = Animator.StringToHash("Retreat");
    private readonly int pushAnimationHash = Animator.StringToHash("Push");
    private readonly int dashAnimationHash = Animator.StringToHash("Dash");
    private readonly int sideStepAnimationHash = Animator.StringToHash("SideStep");
    private int currentAnimationHash;


    public PlayerStateMachine(Transform playerRoot)
    {
        anim = playerRoot.GetComponent<Animator>();
        anim.enabled = false;
    }

    /// <summary>
    /// Resolve attack connection on both attacker and defender, with <paramref name="isDefender"/> as a filter flag
    /// </summary>
    public void ResolveAttack(AttackData attack, AttackResult result, PlayerAttackHandler attackHandler, bool isDefender)
    {
        switch (result)
        {
            case AttackResult.Parried:
                TimeStop = GameRules.CombatSettings.Parry.HitStop;
                if (!isDefender)
                {
                    attackHandler.OnStunned();
                    HitStun = GameRules.CombatSettings.Parry.HitStun;
                }
                break;

            case AttackResult.Hit or AttackResult.CounterHit:

                TimeStop = attack.FrameData.HitStop;
                if (isDefender)
                {
                    attackHandler.OnStunned();
                    HitStun = result == AttackResult.CounterHit ?
                        attack.FrameData.CounterHitStun :
                        attack.FrameData.HitStun;
                }
                break;

            case AttackResult.StandingBlocked or AttackResult.LowBlocked:
                TimeStop += attack.FrameData.BlockStop;
                if (isDefender)
                {
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
    }

    /// <summary>
    /// Update animator and tick down stun states
    /// </summary>
    public void TickUpdate()
    {
        if (TimeStop > 0)
        {
            anim.speed = 0;
            TimeStop -= 1;
            return;
        }

        Recovery = Mathf.Clamp(Recovery - 1, 0, int.MaxValue);
        HitStun = Mathf.Clamp(HitStun - 1, 0, int.MaxValue);
        BlockStun = Mathf.Clamp(BlockStun - 1, 0, int.MaxValue);

        // If player was stunned or recovering and just recovered, set state to buffered state
        if (HitStun == 0 && BlockStun == 0 && Recovery == 0 &&
            (State.CombatState == CombatState.HitStun || State.CombatState == CombatState.BlockStun))
        {
            SetCombatState(CombatState.Idle);
        }

        if (IsInActionLock == false)
        {
            int animHash;
            int frameBlend;
            if (State.StanceState == StanceState.Crouching)
            {
                animHash = crouchingAnimationHash;
                frameBlend = 3;
            }
            else
            {
                (animHash, frameBlend) = State.MovementState switch
                {
                    MovementState.Idle => (idleAnimationHash, 6),
                    MovementState.Retreating => (retreatAnimationHash, 3),

                    MovementState.Pushing => (pushAnimationHash, 3),
                    MovementState.Dashing => (dashAnimationHash, 5),
                    MovementState.SideStepping => (sideStepAnimationHash, 5),

                    _ => (Animator.StringToHash("Missing"), -1),
                };
            }

            PlayAnimation(animHash, frameBlend);
        }

        anim.speed = 1;
        anim.Update(GlobalGameData.TICK_TIME);
    }
    public void PlayAnimation(int animHash, int transitionFrames = 0, int layer = 0)
    {
        if (currentAnimationHash == animHash) return;

        currentAnimationHash = animHash;

        if (transitionFrames == 0)
        {
            anim.PlayInFixedTime(animHash, layer);
        }
        else
        {
            anim.CrossFadeInFixedTime(animHash, transitionFrames * GlobalGameData.TICK_TIME, layer);
        }
    }
}