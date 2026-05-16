using UnityEngine;


/// <summary>
/// MB class that is the core of the player controller. Holds subcomponenents and handles input.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerController opponent;
    [SerializeField] private AttackMoveSetSO moveSetSO;
    [SerializeField] private bool isRightPlayer;

    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerInputHandler inputHandler;

    [SerializeField] private PlayerAttackHandler attackHandler;
    [SerializeField] private PlayerMovementHandler movementHandler;

    [SerializeField] private PlayerColliderHandler colliderHandler;
    [EditorReadOnly, SerializeField] private PlayerInputRouter inputRouter;

    public PlayerColliderHandler ColliderHandler => colliderHandler;
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


    private void Awake()
    {
        GameRules.PostRulesInitialized += () =>
        {
            colliderHandler = new PlayerColliderHandler(transform);

            inputHandler = new PlayerInputHandler(moveSetSO.GetAttacksArray());
            stateMachine = new PlayerStateMachine(transform, isRightPlayer);

            movementHandler = new PlayerMovementHandler(stateMachine, inputHandler, transform, isRightPlayer);
            attackHandler = new PlayerAttackHandler(stateMachine, inputHandler, colliderHandler);

            if (TryGetComponent(out inputRouter))
            {
                inputRouter.Init(inputHandler);
            }
        };
    }

    public void OnUpdate(float deltaTime)
    {
        movementHandler.OnUpdate(deltaTime);
    }

    public void PreTickUpdate()
    {
        // Push all collected inputs into tick buffer.
        inputHandler.CollectInputs();

        colliderHandler.RecalculateHitBoxes();
    }
    public void TickUpdate()
    {
        // If player is not in the attack active state, return
        if (stateMachine.State.CombatState != CombatState.AttackActive) return;

        // Check if a possible active attack hit the opponent. (attackers perspective)
        if (attackHandler.CheckAttackIntersection(opponent, out AttackData activeAttack))
        {
            attackHandler.OnActiveAttackConnected();

            AttackResult result = opponent.attackHandler.GetInboundAttackResult(activeAttack.Level);

            //DebugLogger.Log("hit: " + result);

            stateMachine.ResolveAttack(activeAttack, result, attackHandler, false);
            opponent.stateMachine.ResolveAttack(activeAttack, result, opponent.attackHandler, true);
        }
    }
    public void PostTickUpdate()
    {
        // Check if player is allowed to do an action before attack tickupdate
        attackHandler.TickUpdateAttackSequence();
        
        // And check it again after attack tickupdate
        if (!stateMachine.IsInActionLock)
        {
            movementHandler.TickUpdateMovement();
        }

        // Tick down stuns and recovery
        stateMachine.TickUpdate();
    }
}