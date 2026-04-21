using UnityEngine;


/// <summary>
/// MB class that is the core of the player controller. Holds subcomponenents and handles input.
/// </summary>
public class PlayerController : FrameTickMonoBehaviour
{
    [SerializeField] private PlayerController opponent;
    [SerializeField] private AttackMoveSetSO moveSetSO;

    [SerializeField] private PlayerStateMachine stateMachine = new();
    [SerializeField] private PlayerInputHandler inputHandler = new();
    [SerializeField] private PlayerColliderHandler collisionHandler;

    public PlayerInputHandler InputHandler => inputHandler;

    public bool IsAssigned;


    private void Awake()
    {
        inputHandler.Init(moveSetSO.GetMoveArray());
        stateMachine.Init(transform);
        collisionHandler = new PlayerColliderHandler(transform);
    }


    protected override void OnFrameTick()
    {
        inputHandler.CollectInputs();
        if (stateMachine.IsStunned == false)
        {
            if (inputHandler.TryGetMove(out AttackData targetMove))
            {
                print(targetMove.AnimationName);
            }
        }

        // If any move is active from this player (attackers perpective), check collision between any active hurtboxes with the opponennts hitboxes.
        if (stateMachine.State == FighterState.MoveActive)
        {
            // Check if any opponent hitbix is hit
            if (CollisionUtils.CheckAABBIntersection(opponent.collisionHandler.HitBoxes, collisionHandler.HurtBoxes))
            {
                // hit opponent and send Attack Level (Low/Mid/High)
                opponent.OnAttackImpact(stateMachine.CurrentMove.Level);
            }
        }

        bool stunned = stateMachine.IsStunned;

        stateMachine.OnFrameTick();
    }

    /// <summary>
    /// Called when this player (from defender perspective) gets hit by an attack.
    /// </summary>
    public void OnAttackImpact(AttackLevel level)
    {
        GuardResult guardResult = CollisionUtils.GetGuardResult(level, stateMachine.State);
    }
}