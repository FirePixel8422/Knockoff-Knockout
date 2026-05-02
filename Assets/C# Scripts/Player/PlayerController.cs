using UnityEngine;


/// <summary>
/// MB class that is the core of the player controller. Holds subcomponenents and handles input.
/// </summary>
public class PlayerController : FrameTickMonoBehaviour
{
    [SerializeField] private PlayerController opponent;
    [SerializeField] private AttackMoveSetSO moveSetSO;

    [SerializeField] private PlayerAttackHandler attackHandler;
    [SerializeField] private PlayerMovementHandler movementHandler;

    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private PlayerColliderHandler collisionHandler;

    public PlayerInputHandler InputHandler => inputHandler;

    public bool IsAssigned;


    private void Awake()
    {
        inputHandler = new PlayerInputHandler(moveSetSO.GetAttacksArray());
        stateMachine = new PlayerStateMachine(transform);
        collisionHandler = new PlayerColliderHandler(transform);
    }


    // Core tick loop.
    protected override void OnFrameTick()
    {
        inputHandler.CollectInputs();
        if (stateMachine.IsStunned == false)
        {
            if (inputHandler.TryReadAttack(out AttackData targetMove))
            {
                attackHandler.SetBufferedAttack(targetMove);
            }
        }

        // If any move is active from this player (attackers perpective), check collision between any active hurtboxes with the opponennts hitboxes.
        if (stateMachine.State.CombatState == CombatState.AttackActive)
        {
            // Check if any opponent hitbix is hit
            if (CollisionUtils.CheckAABBIntersection(opponent.collisionHandler.HitBoxes, collisionHandler.HurtBoxes))
            {
                // hit opponent and send Attack Level (Low/Mid/High)
                //GuardResult opponentGuardResult = opponent.OnAttackImpact(stateMachine.CurrentMove.Level);
            }
        }

        bool stunned = stateMachine.IsStunned;

        stateMachine.OnFrameTick();
    }

    public void StartAttack(AttackData targetMove)
    {

    }

    /// <summary>
    /// Called when this player (from defender perspective) gets hit by an attack.
    /// </summary>
    public GuardResult OnAttackImpact(AttackLevel level)
    {
        GuardResult guardResult = GetGuardResult(level, stateMachine.State);
        return guardResult;

        // CLEAR BUFFERED MOVE
        // CLEAR BUFFERED MOVE
        // CLEAR BUFFERED MOVE
        // CLEAR BUFFERED MOVE
        // CLEAR BUFFERED MOVE
    }

    /// <summary>
    /// Get GuardResult based on what type of attack hit the target player in what FighterState
    /// </summary>
    private GuardResult GetGuardResult(AttackLevel attackType, FighterState defenderState)
    {
        bool defenderIsInStartup = defenderState.CombatState == CombatState.AttackStartup;

        // If defender is still stunned or if the incoming attack is unblockable, defender gets hit OR interrupted
        if (defenderState.CombatState == CombatState.HitStun || attackType == AttackLevel.Unblockable)
        {
            return defenderIsInStartup ?
                GuardResult.Interrupted :
                GuardResult.Hit;
        }
        // If defender is in startup animation of their own attack, they gets interrupted
        if (defenderIsInStartup)
        {
            return GuardResult.Interrupted;
        }

        return defenderState.MovementState switch
        {
            // If defender is standing still or walking backwards, they blocks mids and highs and lose to lows
            MovementState.Standing or MovementState.Retreating =>
                attackType switch
                {
                    AttackLevel.Mid or AttackLevel.High =>
                        GuardResult.StandingBlocked,

                    _ =>
                        GuardResult.Hit,
                },

            // If defender is crouching, they blocks Lows and lose to mids and highs
            MovementState.Crouching =>
                attackType switch
                {
                    AttackLevel.Low =>
                        GuardResult.LowBlocked,

                    _ =>
                        GuardResult.Hit,
                },

            _ =>
                GuardResult.Hit,
        };
    }
}