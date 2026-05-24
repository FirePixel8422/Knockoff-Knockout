using UnityEngine;


/// <summary>
/// MB class that is the core of the player controller. Holds subcomponenents and handles input.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerController opponent;
    [SerializeField] private AttackMoveSetSO moveSetSO;
    [SerializeField] private bool isLeftPlayer;

    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerInputHandler inputHandler;

    [SerializeField] private PlayerColliderHandler colliderHandler;
    [SerializeField] private PlayerHealthHandler healthHandler;

    [SerializeField] private PlayerAttackHandler attackHandler;
    [SerializeField] private PlayerMovementHandler movementHandler;

    [EditorReadOnly, SerializeField] private PlayerInputRouter inputRouter;

    public PlayerInputHandler InputHandler => inputHandler;
    public PlayerColliderHandler ColliderHandler => colliderHandler;
    public PlayerHealthHandler HealthHandler => healthHandler;
    public PlayerMovementHandler MovementHandler => movementHandler;
    public PlayerInputRouter InputRouter => inputRouter;



#if UNITY_EDITOR
    [SerializeField] private bool drawHitBoxGizmos;
    [SerializeField] private bool drawHurtBoxGizmos;

    private void OnValidate()
    {
        FastHitBox[] playerHitBoxes = GetComponentsInChildren<FastHitBox>(true);

        int hitBoxCount = playerHitBoxes.Length;
        for (int i = 0; i < hitBoxCount; i++)
        {
            FastHitBox hitBox = playerHitBoxes[i];
            if (hitBox is FastHurtBox hurtBox)
            {
                hurtBox.DrawGizmos = drawHurtBoxGizmos;
            }
            else
            {
                hitBox.DrawGizmos = drawHitBoxGizmos;
            }
        }
    }
#endif


    public void Init()
    {
        stateMachine = new PlayerStateMachine(transform);
        inputHandler = new PlayerInputHandler(moveSetSO.GetBakedDataArray());

        colliderHandler = new PlayerColliderHandler(transform);
        healthHandler = new PlayerHealthHandler(ref stateMachine.OnDamageTaken);

        movementHandler = new PlayerMovementHandler(stateMachine, inputHandler, transform, isLeftPlayer);
        attackHandler = new PlayerAttackHandler(stateMachine, inputHandler, colliderHandler, movementHandler);

        if (TryGetComponent(out inputRouter))
        {
            inputRouter.Init(inputHandler);
        }
    }

    public void OnUpdate(float deltaTime)
    {
        if (stateMachine.IsTimeStopped) return;

        movementHandler.OnUpdate(deltaTime);
    }

    public void PreTickUpdate()
    {
        if (stateMachine.IsTimeStopped) return;

        // Push all collected inputs into tick buffer.
        inputHandler.CollectInputs();

        colliderHandler.RecalculateHitBoxes();
    }
    public void TickUpdate()
    {
        if (stateMachine.IsTimeStopped) return;

        // If player is not in the attack active state, return
        if (stateMachine.State.CombatState != CombatState.AttackActive) return;

        // Check if a possible active attack hit the opponent. (attackers perspective)
        if (attackHandler.CheckAttackIntersection(opponent, out AttackData activeAttack))
        {
            attackHandler.OnActiveAttackConnected();

            AttackResult result = opponent.attackHandler.GetInboundAttackResult(activeAttack.Level);

            stateMachine.ResolveAttack(activeAttack, result, false);
            opponent.stateMachine.ResolveAttack(activeAttack, result, true);
        }
    }
    public void PostTickUpdate()
    {
        if (!stateMachine.IsTimeStopped)
        {
            bool wasInActionRecovery = stateMachine.State.CombatState == CombatState.Recovering;

            if (!stateMachine.IsStunned)
            {
                // TickUpdate attack (updating a seq or reading the input buffer for a new attack)
                attackHandler.TickUpdateAttackSequence(stateMachine.IsInCombatLock);
            }

            // TickUpdate any active movement action and read movement input IF player wont be actionlocked next frame
            bool isActionLocked = wasInActionRecovery || stateMachine.IsInCombatLock || stateMachine.IsInMoveLock;
            movementHandler.TickUpdateMovement(isActionLocked);
        }

        // Tick down stuns and recovery
        stateMachine.TickUpdate();
    }
}