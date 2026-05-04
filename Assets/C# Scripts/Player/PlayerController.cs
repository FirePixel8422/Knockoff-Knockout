using UnityEngine;


/// <summary>
/// MB class that is the core of the player controller. Holds subcomponenents and handles input.
/// </summary>
public class PlayerController : FrameTickUpdateMB
{
    [SerializeField] private PlayerController opponent;
    [SerializeField] private AttackMoveSetSO moveSetSO;

    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerInputHandler inputHandler;

    [SerializeField] private PlayerAttackHandler attackHandler;
    [SerializeField] private PlayerMovementHandler movementHandler;

    [SerializeField] private PlayerColliderHandler collisionHandler;

    public PlayerInputHandler InputHandler => inputHandler;
    public PlayerMovementHandler MovementHandler => movementHandler;


    private void Awake()
    {
        inputHandler = new PlayerInputHandler(moveSetSO.GetAttacksArray());
        stateMachine = new PlayerStateMachine(transform);

        movementHandler = new PlayerMovementHandler(inputHandler, stateMachine, transform);
        attackHandler = new PlayerAttackHandler(inputHandler, stateMachine);

        collisionHandler = new PlayerColliderHandler(transform);

        GetComponent<PlayerInputRouter>().Init(inputHandler);
    }

    protected override void OnUpdate()
    {
        movementHandler.OnUpdate();
    }

    // Core tick loop.
    protected override void OnFrameTick()
    {
        // 1: Collect Inputs
        inputHandler.CollectInputs();

        // 2: Run Attack Systems
        attackHandler.OnFrameTick();

        // 3: Run Movement Systems
        movementHandler.OnFrameTick();

        if (stateMachine.IsStunned == false)
        {
        }
        // 4: Run Attack Collision
        // If any move is active from this player (attackers perpective), check collision between any active hurtboxes with the opponennts hitboxes.
        if (stateMachine.State.CombatState == CombatState.AttackActive)
        {
            // Check if any opponent hitbix is hit
            if (CollisionUtils.CheckAABBIntersection(opponent.collisionHandler.HitBoxes, collisionHandler.HurtBoxes))
            {
                // hit opponent and send Attack Level (Low/Mid/High)
                AttackResult opponentGuardResult = opponent.OnAttackImpact(attackHandler.CurrentActiveAttack.Level);
            }
        }

        bool stunned = stateMachine.IsStunned;

        stateMachine.OnFrameTick();
    }

    /// <summary>
    /// Called when this player (from defender perspective) gets hit by an attack.
    /// </summary>
    public AttackResult OnAttackImpact(AttackLevel level)
    {
        AttackResult attackResult = GetAttackResult(level, stateMachine.State);
        return attackResult;

        // CLEAR BUFFERED MOVE
        // CLEAR BUFFERED MOVE
        // CLEAR BUFFERED MOVE
        // CLEAR BUFFERED MOVE
        // CLEAR BUFFERED MOVE
    }

    /// <summary>
    /// Get <see cref="AttackResult"/> based on what type of attack hit the defender in what state
    /// </summary>
    private AttackResult GetAttackResult(AttackLevel attackType, FighterState defenderState)
    {
        // If the defender is in an active parry
        if (defenderState.CombatState == CombatState.ParryHigh)
        {
            return attackType == AttackLevel.High
                ? AttackResult.Parried
                : AttackResult.Hit;
        }
        if (defenderState.CombatState == CombatState.ParryLow)
        {
            return attackType == AttackLevel.Low
                ? AttackResult.Parried
                : AttackResult.Hit;
        }

        // If defender cant block or the incoming attack is unblockable, the defender gets hit OR interrupted
        if ((defenderState.CanBlock() == false) || attackType == AttackLevel.Unblockable)
        {
            return defenderState.CombatState == CombatState.AttackStartup ?
                AttackResult.CounterHit :
                AttackResult.Hit;
        }

        // If defender is crouching, they blocks lows, duck highs but lose to mids
        if (defenderState.GroundState == GroundState.Crouching)
        {
            return attackType switch
            {
                AttackLevel.Low =>
                 AttackResult.LowBlocked,

                AttackLevel.High => 
                    AttackResult.Missed,
                _ =>
                    AttackResult.Hit,
            };
        }
        // If defender is standing
        if (defenderState.GroundState == GroundState.Standing)
        {
            return defenderState.MovementState switch
            {
                // If defender is standing still or walking backwards, they blocks mids and highs and lose to lows
                MovementState.Idle or MovementState.Retreating =>
                    attackType switch
                    {
                        AttackLevel.Mid or AttackLevel.High =>
                            AttackResult.StandingBlocked,

                        _ =>
                            AttackResult.Hit,
                    },

                _ => 
                    AttackResult.Hit,
            };
        }

        // Should be unreachable, so parry to allow quicker debugging.
        return AttackResult.Parried;
    }
}