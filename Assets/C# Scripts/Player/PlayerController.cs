using UnityEngine;


/// <summary>
/// MB class that is the core of the player controller. Holds subcomponenents and handles input.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerController opponent;
    [SerializeField] private bool isLeftPlayer;

    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerInputHandler inputHandler;

    [SerializeField] private PlayerColliderHandler colliderHandler;
    [SerializeField] private PlayerHealthHandler healthHandler;

    [SerializeField] private PlayerMovementHandler movementHandler;
    [SerializeField] private PlayerAttackHandler attackHandler;

    [SerializeField] private PlayerParticleHandler particleHandler;

    [EditorReadOnly, SerializeField] private PlayerInputRouter inputRouter;
    [EditorReadOnly, SerializeField] private PlayerInputOverrider inputOverrider;

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


    public void Init(AttackData[] attackMoveSet)
    {
        stateMachine = new PlayerStateMachine(transform);
        inputHandler = new PlayerInputHandler(attackMoveSet);

        colliderHandler = new PlayerColliderHandler(transform);
        healthHandler = new PlayerHealthHandler(ref stateMachine.OnDamageTaken);

        movementHandler = new PlayerMovementHandler(stateMachine, inputHandler, transform, isLeftPlayer);
        attackHandler = new PlayerAttackHandler(stateMachine, inputHandler, colliderHandler, movementHandler);

        particleHandler.Init(ref stateMachine.OnAttackConnected);

        if (TryGetComponent(out inputRouter))
        {
            inputRouter.Init(inputHandler);
        }
        inputOverrider = GetComponent<PlayerInputOverrider>();
    }

    public void OnUpdate(float deltaTime)
    {
        movementHandler.OnUpdate(deltaTime);
    }

    public void PreTickUpdate()
    {
        stateMachine.TickUpdateStuns();

        // Push all collected inputs into tick buffer.
        inputOverrider.CollectInputs();
        inputHandler.CollectInputs();

        colliderHandler.RecalculateHitBoxes();


        attackHandler.TickUpdateAttackSequence();
        movementHandler.TickUpdateMoveSequence();

        bool wasInActionRecovery = stateMachine.State.CombatState == CombatState.Recovering;
        if (!stateMachine.IsInCombatLock)
        {
            // TickUpdate attack (updating a seq or reading the input buffer for a new attack)
            attackHandler.TickUpdateAttackInput();
        }

        // TickUpdate any active movement action and read movement input IF player wont be actionlocked next frame
        bool isActionLocked = wasInActionRecovery || stateMachine.IsInCombatLock || stateMachine.IsInMoveLock;
        if (!isActionLocked)
        {
            movementHandler.TickUpdateMoveInput();
        }
    }

    public void TickUpdate(out bool activeAttackConnected)
    {
        activeAttackConnected = false;

        // If player is not in the attack active state, return
        if (!stateMachine.IsAttackActive) return;

        // Check if a possible active attack hit the opponent. (attackers perspective)
        activeAttackConnected = attackHandler.CheckAttackIntersection(opponent);
    }
    
    public void PostTickUpdate(bool activeAttackConnected)
    {
        if (activeAttackConnected)
        {
            AttackData activeAttack = attackHandler.ActiveAttack;
            AttackResult attackResult = opponent.attackHandler.GetInboundAttackResult(activeAttack.Level, activeAttack.DoesKnockdown);

            attackHandler.OnActiveAttackConnected();

            stateMachine.ResolveAttack(activeAttack, attackResult, false);
            opponent.stateMachine.ResolveAttack(activeAttack, attackResult, true);
        }

        // Tick down stuns and recovery
        stateMachine.TickUpdateAnimator();
    }
}