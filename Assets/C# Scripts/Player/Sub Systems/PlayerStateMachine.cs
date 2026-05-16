using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for handling player state
/// </summary>
[System.Serializable]
public class PlayerStateMachine
{
    private readonly Animator anim;
    private readonly bool isRightPlayer;

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


    private readonly int standingHurtAnimHash = Animator.StringToHash("StandingHurt");
    private readonly int crouchingHurtAnimHash = Animator.StringToHash("CrouchingHurt");

    private readonly int standingBlockAnimHash = Animator.StringToHash("StandingBlock");
    private readonly int crouchingBlockAnimHash = Animator.StringToHash("CrouchingBlock");

    private readonly int crouchingAnimHash = Animator.StringToHash("Crouching");
    private readonly int idleAnimHash = Animator.StringToHash("Idle");
    private readonly int retreatAnimHash = Animator.StringToHash("Retreat");

    private readonly int pushAnimHash = Animator.StringToHash("Push");
    private readonly int dashAnimHash = Animator.StringToHash("Dash");
    private readonly int sideStepAnimHash = Animator.StringToHash("SideStep");
    private int currentAnimHash;


    public PlayerStateMachine(Transform playerRoot, bool isRightPlayer)
    {
        anim = playerRoot.GetComponent<Animator>();
        anim.enabled = false;
        this.isRightPlayer = isRightPlayer;
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
                    PlayAnimation(attack.Level == AttackLevel.Low ?
                        crouchingHurtAnimHash :
                        standingHurtAnimHash);

                    HitStun = result == AttackResult.CounterHit ?
                        attack.FrameData.CounterStun :
                        attack.FrameData.HitStun;

                    HUDManager.Instance.GetPlayerUIModule(isRightPlayer).HealthBar.TakeDamage(attack.Damage);
                    attackHandler.OnStunned();
                }
                break;

            case AttackResult.StandingBlocked or AttackResult.LowBlocked:

                TimeStop = attack.FrameData.BlockStop;
                if (isDefender)
                {
                    PlayAnimation(result == AttackResult.StandingBlocked ?
                        standingBlockAnimHash :
                        crouchingBlockAnimHash);
                    
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
    }


    /// <summary>
    /// Advance current animation by <paramref name="tickAdvanceCount"/> amount of ticks
    /// </summary>
    public void TickAdvanceAnimation(int tickAdvanceCount)
    {
        anim.Update(GlobalGameData.TICK_TIME * tickAdvanceCount);
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
                animHash = crouchingAnimHash;
                frameBlend = 3;
            }
            else
            {
                (animHash, frameBlend) = State.MovementState switch
                {
                    MovementState.Idle => (idleAnimHash, 6),
                    MovementState.Retreating => (retreatAnimHash, 3),

                    MovementState.Pushing => (pushAnimHash, 3),
                    MovementState.Dashing => (dashAnimHash, 5),
                    MovementState.SideStepping => (sideStepAnimHash, 5),

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
        if (currentAnimHash == animHash) return;

        currentAnimHash = animHash;

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