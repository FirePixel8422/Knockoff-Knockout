using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for player movement
/// </summary>
[System.Serializable]
public class PlayerMovementHandler
{
    private readonly PlayerInputHandler inputHandler;
    private readonly PlayerStateMachine stateMachine;
    private readonly Transform transform;
    private readonly bool isRightPlayer;

    private readonly float moveSpeed;
    private readonly float moveSnappyness;
    private readonly SideStepSettings sideStepSettings;
    private readonly DashSettings dashSettings;

    private Vector3 targetFighterPosition;
    private Vector3 lastMoveDir;
    public Vector3 LastMoveDir => lastMoveDir;

    private ActionSequence<MovementState> currentSequence;


    public PlayerMovementHandler(PlayerStateMachine stateMachine, PlayerInputHandler inputHandler, Transform transform, bool isRightPlayer)
    {
        this.stateMachine = stateMachine;
        this.inputHandler = inputHandler;
        this.transform = transform;
        this.isRightPlayer = isRightPlayer;

        stateMachine.OnKnockbackTaken += AddKnockBack;
        stateMachine.OnStunned += OnStunned;

        targetFighterPosition = transform.position;

        moveSpeed = GameRules.CombatSettings.Fighter.MoveSpeed;
        moveSnappyness = GameRules.CombatSettings.Fighter.MoveSnappyness;
        sideStepSettings = GameRules.CombatSettings.SideStep;
        dashSettings = GameRules.CombatSettings.Dash;

        currentSequence = new ActionSequence<MovementState>((MovementState.Idle, 0));
    }
    private PlayerMovementHandler() { }


    /// <summary>
    /// Check for movement input in current tick buffer and resolve it
    /// </summary>
    public void TickUpdateMovement(bool isActionLocked)
    {
        lastMoveDir = Vector3.zero;

        if (currentSequence.IsActive)
        {
            UpdateActiveMoveAction();
            return;
        }
        if (isActionLocked) return;

        ReadAndApplyNewInput();
    }

    /// <summary>
    /// TickUpdate active movement sequence and send state changes to the <see cref="PlayerStateMachine"/>. Also updates player transform data based on the type of movement action (Sidestep/Dash).
    /// </summary>
    private void UpdateActiveMoveAction()
    {
        // Update sequence and check state change
        if (currentSequence.TickUpdateState(out MovementState newState, out int elapsedSequenceTicks))
        {
            stateMachine.SetMovementState(newState);

            if (newState == MovementState.Recovery)
            {
                stateMachine.SetCombatState(CombatState.Recovering);
            }
            else if (newState == MovementState.Idle)
            {
                stateMachine.SetCombatState(CombatState.Idle);
            }
            return;
        }

        // If state didnt change:
        if (stateMachine.State.MovementState == MovementState.SideSteppingDown ||
            stateMachine.State.MovementState == MovementState.SideSteppingUp)
        {
            // Rotate around other player as orbit.
        }
        else if (stateMachine.State.MovementState == MovementState.DashingBack ||
                stateMachine.State.MovementState == MovementState.DashingForward)
        {
            bool isDashForward = stateMachine.State.MovementState == MovementState.DashingForward;

            float t = (float)elapsedSequenceTicks / dashSettings.Duration;
            float tPrev = (float)(elapsedSequenceTicks - 1) / dashSettings.Duration;

            float prevForce = dashSettings.Curve.Evaluate(tPrev);
            float currentForce = dashSettings.Curve.Evaluate(t);

            AddForce((currentForce - prevForce) * (isDashForward ? dashSettings.Power : -dashSettings.Power));
        }

    }
    private void ReadAndApplyNewInput()
    {
        // If fighter sidesteps, set fighter in standing stance and sidestepping movement state
        if (inputHandler.TryReadSideStep(out bool isSideStepUp))
        {
            MovementState targetSideStep = isSideStepUp ? MovementState.SideSteppingUp : MovementState.SideSteppingDown;

            stateMachine.SetStanceState(StanceState.Standing);
            stateMachine.SetMovementState(targetSideStep);
            stateMachine.SetCombatState(CombatState.ActionStartup);

            currentSequence = new ActionSequence<MovementState>(
                (targetSideStep, sideStepSettings.Duration),
                (MovementState.Recovery, sideStepSettings.Recovery),
                (MovementState.Idle, 0));

            UpdateMoveActionAnimation(targetSideStep);
            return;
        }
        // If fighter dashes, set fighter in standing stance and dashing movement state
        if (inputHandler.TryReadDash(out bool isDashForward))
        {
            MovementState targetDash = isDashForward ? MovementState.DashingForward : MovementState.DashingBack;

            stateMachine.SetStanceState(StanceState.Standing);
            stateMachine.SetMovementState(targetDash);
            stateMachine.SetCombatState(CombatState.ActionStartup);

            currentSequence = new ActionSequence<MovementState>(
                (targetDash, dashSettings.Duration),
                (MovementState.Recovery, dashSettings.Recovery),
                (MovementState.Idle, 0));

            UpdateMoveActionAnimation(targetDash);

            return;
        }

        // If fighter doesnt do a special movement action (Sidestep/Dash), check for normal movement input and resolve it
        switch (inputHandler.GetCurrentDirection())
        {
            // Crouch Idle
            case DirectionInput.Down:
                {
                    stateMachine.SetStanceState(StanceState.Crouching);
                    stateMachine.SetMovementState(MovementState.Idle);
                    break;
                }
            // Standing Moving
            case DirectionInput.Left:
                {
                    stateMachine.SetStanceState(StanceState.Standing);
                    stateMachine.SetMovementState(isRightPlayer ? MovementState.Pushing : MovementState.Retreating);
                    MovePlayer(-moveSpeed * GlobalGameData.TICK_TIME * CameraManager.Instance.GetForwardDir());
                    break;
                }
            // Standing Moving
            case DirectionInput.Right:
                {
                    stateMachine.SetStanceState(StanceState.Standing);
                    stateMachine.SetMovementState(isRightPlayer ? MovementState.Retreating : MovementState.Pushing);
                    MovePlayer(moveSpeed * GlobalGameData.TICK_TIME * CameraManager.Instance.GetForwardDir());
                    break;
                }
            // Standing Idle
            case DirectionInput.Neutral:
                {
                    stateMachine.SetStanceState(StanceState.Standing);
                    stateMachine.SetMovementState(MovementState.Idle);
                    break;
                }
            default: break;
        }
    }

    /// <summary>
    /// Update fighter animation
    /// </summary>
    private void UpdateMoveActionAnimation(MovementState actionState)
    {
        AnimData animData = actionState switch
        {
            MovementState.DashingBack => AnimHashes.Movement.DashBack,
            MovementState.DashingForward => AnimHashes.Movement.DashForward,

            MovementState.SideSteppingDown => AnimHashes.Movement.SideStepDown,
            MovementState.SideSteppingUp => AnimHashes.Movement.SideStepUp,

            _ => AnimHashes.Missing
        };
        stateMachine.PlayAnimation(animData);
    }


    private void OnStunned()
    {
        currentSequence.Cancel();
    }
    private void AddKnockBack(float kb) => AddForce(-kb);
    public void AddForce(float force)
    {
        MovePlayer(CameraManager.Instance.GetForwardDir() * (isRightPlayer ? -force : force));
    }

    /// <summary>
    /// Add movement to the player's position, which will be lerped to over time in OnUpdate()
    /// </summary>
    public void MovePlayer(Vector3 addedMovement)
    {
        lastMoveDir = addedMovement.normalized;

        targetFighterPosition = CameraManager.Instance.ClampMovementToCameraBounds(targetFighterPosition, addedMovement);
    }
    // Lerp currentFighterPosition to targetFighterPosition
    public void OnUpdate(float deltaTime)
    {
        transform.position = Vector3.Lerp(transform.position, targetFighterPosition, moveSnappyness * deltaTime);
    }
}