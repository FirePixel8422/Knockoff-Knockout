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

    [SerializeField] private PlayerColliderHandler colliderHandler;
    [SerializeField] private PlayerInputRouter inputRouter;

    public PlayerInputHandler InputHandler => inputHandler;
    public PlayerMovementHandler MovementHandler => movementHandler;
    public PlayerColliderHandler ColliderHandler => colliderHandler;
    public PlayerInputRouter InputRouter => inputRouter;


    private void Awake()
    {
        colliderHandler = new PlayerColliderHandler(transform);

        inputHandler = new PlayerInputHandler(moveSetSO.GetAttacksArray());
        stateMachine = new PlayerStateMachine(transform);

        movementHandler = new PlayerMovementHandler(inputHandler, stateMachine, transform);
        attackHandler = new PlayerAttackHandler(inputHandler, stateMachine, colliderHandler);

        if (TryGetComponent(out inputRouter))
        {
            inputRouter.Init(inputHandler);
        }
    }

    protected override void OnUpdate()
    {
        if (MatchManager.Instance.GamePaused) return;

        movementHandler.OnUpdate();
    }

    public void PreTickUpdate()
    {
        // Check if a possible active attack hit the opponent. (attackers perspective)
        if (attackHandler.CheckAttackIntersection(opponent, out AttackResult result))
        {

        }
    }
    /// <summary>
    /// Called when this fighter (defender perspective) gets hit by an attack.
    /// </summary>
    public AttackResult OnAttackImpact(AttackLevel level)
    {
        AttackResult attackResult = PlayerAttackHandler.GetAttackResult(level, stateMachine.State);

        // When fighter gets hit by an attack, clear their input buffer to avoid unintended buffered inputs after hitstun wears off.
        // This to ensure the player doesnt accidentally do a buffer spammed that makes them even more vulnerable after getting hit.
        inputHandler.ClearInputBuffer();

        return attackResult;
    }
    public void TickUpdate()
    {
        attackHandler.TickUpdateAttackSequence();

        movementHandler.TickUpdateMovement();
    }

    public void PostTickUpdate()
    {

    }

    //// Core tick loop.
    //protected override void OnTickUpdate()
    //{
    //    if (MatchManager.Instance.GamePaused) return;

    //    // 1: Collect Inputs
    //    inputHandler.CollectInputs();

    //    // 2: Run Attack Systems
    //    attackHandler.TickUpdate();

    //    // 3: Run Movement Systems
    //    movementHandler.TickUpdate();

    //    if (stateMachine.IsStunned == false)
    //    {
    //    }


    //    bool stunned = stateMachine.IsStunned;

    //    stateMachine.TickUpdate();
    //}
}