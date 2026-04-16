using UnityEngine;


/// <summary>
/// MB class that is the core of the player controller. Holds subcomponenents and handles input.
/// </summary>
public class PlayerController : FrameTickMonoBehaviour
{
    [SerializeField] private PlayerController opponent;
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerColliderHandler collisionHandler;
    [SerializeField] private PlayerInputHandler inputHandler;

    public PlayerInputHandler InputHandler => inputHandler;

    public bool IsAssigned;


    private void Awake()
    {
        collisionHandler.Init(transform);
    }


    protected override void OnFrameTick()
    {
        if (stateMachine.State == FighterState.MoveActive)
        {
            if (CollisionUtils.CheckAABBIntersection(opponent.collisionHandler.HitBoxes, collisionHandler.HurtBoxes))
            {
                GuardResult guardResult = CollisionUtils.GetGuardResult(stateMachine.CurrentMove.Type, opponent.stateMachine.State);
            }
        }

        bool stunned = stateMachine.IsStunned;

        inputHandler.OnFrameTick();
    }
}