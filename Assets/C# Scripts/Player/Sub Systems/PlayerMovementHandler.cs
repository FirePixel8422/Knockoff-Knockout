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
    private readonly bool isLeftPlayer;

    public readonly float pushingSpeed;
    public readonly float retreatingSpeed;

    private readonly float turnSnappyness;
    private readonly float moveSnappyness;
    private readonly SideStepSettings sideStepSettings;
    private readonly DashSettings dashSettings;

    private Vector3LerpState positionState;
    private QuaternionLerpState rotationState;
    public Vector3 CurrentTransformPos => positionState.Current;

    private Vector3 lastMoveDir;
    public Vector3 LastMoveDir => lastMoveDir;

    private ActionSequenceTimeline<MovementState> sequenceTimeline;
    private StateSequence<MovementState> sideStepSequence;
    private StateSequence<MovementState> dashSequence;

    private float currentSideStepPower;


    public PlayerMovementHandler(PlayerStateMachine stateMachine, PlayerInputHandler inputHandler, Transform transform, bool isLeftPlayer)
    {
        this.stateMachine = stateMachine;
        this.inputHandler = inputHandler;
        this.transform = transform;
        this.isLeftPlayer = isLeftPlayer;

        stateMachine.OnKnockbackTaken += AddKnockBack;
        stateMachine.OnStunned += OnStunned;

        positionState = new Vector3LerpState(transform.position, transform.position);
        rotationState = new QuaternionLerpState(transform.rotation, transform.rotation);

        pushingSpeed = GameRules.CombatSettings.Fighter.PushingSpeed;
        retreatingSpeed = GameRules.CombatSettings.Fighter.RetreatingSpeed;

        moveSnappyness = GameRules.CombatSettings.Fighter.MoveSnappyness;
        turnSnappyness = GameRules.CombatSettings.Fighter.TurnSnappyness;

        sideStepSettings = GameRules.CombatSettings.SideStep;
        dashSettings = GameRules.CombatSettings.Dash;

        sequenceTimeline = new ActionSequenceTimeline<MovementState>(new ((MovementState.Idle, 0)));

        sideStepSequence = new StateSequence<MovementState>(
            (MovementState.SideSteppingDown, sideStepSettings.Startup),
            (MovementState.Recovery, sideStepSettings.Recovery),
            (MovementState.Idle, 0));

        dashSequence = new StateSequence<MovementState>(
            (MovementState.DashingBack, dashSettings.Startup),
            (MovementState.Recovery, dashSettings.Recovery),
            (MovementState.Idle, 0));
    }
    private PlayerMovementHandler() { }


    #region Update Special Move Action Sequences

    /// <summary>
    /// TickUpdate active movement sequence and send state changes to the <see cref="PlayerStateMachine"/>. Also updates player transform data based on the type of movement action (Sidestep/Dash).
    /// </summary>
    public void TickUpdateMoveSequence()
    {
        lastMoveDir = Vector3.zero;

        if (!sequenceTimeline.IsActive) return;

        // Update sequence and check state change
        bool didStateChange = sequenceTimeline.TickUpdateState(
            out MovementState newState, 
            out int elapsedSequenceTicks);

        if (didStateChange)
        {
            stateMachine.SetMovementState(newState);

            if (newState == MovementState.Recovery)
            {
                stateMachine.SetCombatState(CombatState.Idle);
            }
            return;
        }

        if (stateMachine.State.MovementState == MovementState.DashingBack ||
            stateMachine.State.MovementState == MovementState.DashingForward)
        {
            UpdateDashSequence(elapsedSequenceTicks);
        }
        else if (stateMachine.State.MovementState == MovementState.SideSteppingDown ||
            stateMachine.State.MovementState == MovementState.SideSteppingUp)
        {
            UpdateSideStepSequence(elapsedSequenceTicks);
        }
    }

    private void UpdateDashSequence(int elapsedSequenceTicks)
    {
        // If sequence elapsedTicks are outside of the dash window, return.
        if (elapsedSequenceTicks > dashSettings.Duration) return;

        bool isDashForward = stateMachine.State.MovementState == MovementState.DashingForward;

        float t = (float)elapsedSequenceTicks / dashSettings.Duration;
        float tPrev = (float)(elapsedSequenceTicks - 1) / dashSettings.Duration;

        float dashForce = dashSettings.Curve.EvaluateDelta(tPrev, t);

        AddForwardForce(dashForce * (isDashForward ? dashSettings.ForwardDashPower : -dashSettings.BackDashPower));
    }
    private void UpdateSideStepSequence(int elapsedSequenceTicks)
    {
        // If sequence elapsedTicks are outside of the dash window, return.
        if (elapsedSequenceTicks > dashSettings.Duration) return;

        bool isSideStepUp = stateMachine.State.MovementState == MovementState.SideSteppingUp;

        float t = (float)elapsedSequenceTicks / sideStepSettings.Duration;
        float tPrev = (float)(elapsedSequenceTicks - 1) / sideStepSettings.Duration;

        float forceStep = sideStepSettings.DurationCurve.EvaluateDelta(tPrev, t);

        AddForwardForce(currentSideStepPower * forceStep * sideStepSettings.ForwardPower);
        AddSidewardForce(currentSideStepPower * forceStep * (isSideStepUp ? -sideStepSettings.SideStepPower : sideStepSettings.SideStepPower));
    }

    #endregion


    #region Read and Apply new Input

    /// <summary>
    /// If the player is not in a move action, check for movement input in current tick buffer and resolve it
    /// </summary>
    public void TickUpdateMoveInput()
    {
        ReadAndApplyNewInput();
    }

    /// <summary>
    /// Check if the input buffer holds input that correspond to a move action, if so start a move sequence. Otherwise calculaet movement and Move- and Stance-State from directional input.
    /// </summary>
    private void ReadAndApplyNewInput()
    {
        // If fighter sidesteps, set fighter in standing stance and sidestepping movement state
        if (inputHandler.TryReadSideStep(out bool isSideStepUp))
        {
            MovementState targetSideStep = isSideStepUp ? MovementState.SideSteppingUp : MovementState.SideSteppingDown;

            stateMachine.SetStanceState(StanceState.Standing);
            stateMachine.SetMovementState(targetSideStep);
            stateMachine.SetCombatState(CombatState.ActionStartup);

            UpdateMoveActionAnimation(targetSideStep);

            sideStepSequence[0] = (targetSideStep, sideStepSettings.Startup);

            sequenceTimeline = new ActionSequenceTimeline<MovementState>(sideStepSequence);

            float playerDistance = CameraManager.Instance.GetPlayerSpacing();
            float dist01 = (playerDistance - sideStepSettings.DistanceRange.min) / (sideStepSettings.DistanceRange.max - sideStepSettings.DistanceRange.min);
            currentSideStepPower = sideStepSettings.PowerCurve.Evaluate(dist01);

            return;
        }

        // If fighter dashes, set fighter in standing stance and dashing movement state
        if (inputHandler.TryReadDash(out bool isDashForward))
        {
            MovementState targetDash = isLeftPlayer == isDashForward ? MovementState.DashingForward : MovementState.DashingBack;

            stateMachine.SetStanceState(StanceState.Standing);
            stateMachine.SetMovementState(targetDash);
            stateMachine.SetCombatState(CombatState.ActionStartup);

            UpdateMoveActionAnimation(targetDash);

            sequenceTimeline = new ActionSequenceTimeline<MovementState>(dashSequence);
            return;
        }


        // If fighter doesnt do a special movement action (Sidestep/Dash), check for normal movement input and resolve it
        DirectionInput dirInput = inputHandler.GetCurrentDirection();
        switch (dirInput)
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
            case DirectionInput.Right:
            {
                bool isMovingRight = dirInput == DirectionInput.Right;
                GetMovementData(isLeftPlayer, isMovingRight, out MovementState moveDirState, out Vector3 moveDelta);

                stateMachine.SetStanceState(StanceState.Standing);
                stateMachine.SetMovementState(moveDirState);

                MovePlayer(moveDelta);
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
    /// Calculate MoveState and MoveDelta based on if target player is moving forward from his perspective.
    /// </summary>
    private void GetMovementData(bool isLeftPlayer, bool isMovingRight, out MovementState moveDir, out Vector3 moveDelta)
    {
        moveDir = isLeftPlayer == isMovingRight ? MovementState.Pushing : MovementState.Retreating;

        float speed = isLeftPlayer ?
            (isMovingRight ? pushingSpeed : -retreatingSpeed) :
            (isMovingRight ? retreatingSpeed : -pushingSpeed);

        moveDelta = speed * GlobalGameData.TICK_TIME * CameraManager.Instance.GetForwardDir();
    }

    /// <summary>
    /// Update fighter animation
    /// </summary>
    private void UpdateMoveActionAnimation(MovementState actionState)
    {
        AnimData animData = actionState switch
        {
            MovementState.DashingBack => GlobalAnimHashes.Movement.Dash.Back,
            MovementState.DashingForward => GlobalAnimHashes.Movement.Dash.Forward,

            MovementState.SideSteppingDown => GlobalAnimHashes.Movement.SideStep.Down,
            MovementState.SideSteppingUp => GlobalAnimHashes.Movement.SideStep.Up,

            _ => GlobalAnimHashes.Missing
        };
        stateMachine.PlayAnimation(animData);
    }

    #endregion


    private void OnStunned()
    {
        sequenceTimeline.Cancel();
        stateMachine.SetMovementState(MovementState.Idle);
    }
    private void AddKnockBack(float kb) => AddForwardForce(-kb);
    public void AddForwardForce(float force)
    {
        MovePlayer(CameraManager.Instance.GetForwardDir() * (isLeftPlayer ? force : -force));
    }
    public void AddSidewardForce(float force)
    {
        MovePlayer(CameraManager.Instance.GetRightDir() * (isLeftPlayer ? force : -force));
    }

    /// <summary>
    /// Add movement to the fighters position, which will be lerped to over time in OnUpdate()
    /// </summary>
    public void MovePlayer(Vector3 addedMovement)
    {
        lastMoveDir = addedMovement.normalized;

        positionState.Target = CameraManager.Instance.ClampMovementToCameraBounds(positionState.Target, addedMovement);
    }
    /// <summary>
    /// Set the fighters target rotation to be so its looking at the other fighter, which is slerped in OnUpdate()
    /// </summary>
    public void RealignFighter()
    {
        rotationState.Target = Quaternion.LookRotation(isLeftPlayer ? CameraManager.Instance.GetForwardDir() : -CameraManager.Instance.GetForwardDir(), Vector3.up);
    }
    /// <summary>
    /// Lerp currentTransformPos to targetTransformPos and update transform position
    /// </summary>
    public void OnUpdate(float deltaTime)
    {
        if (!stateMachine.IsInMoveLock)
        {
            RealignFighter();
        }
        
        transform.SetPositionAndRotation(
            positionState.Lerp(moveSnappyness * deltaTime),
            rotationState.Slerp(turnSnappyness * deltaTime));
    }
}