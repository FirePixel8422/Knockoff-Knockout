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

    private readonly float moveSpeed;
    private readonly float moveSnappyness;

    private Vector3 targetFighterPosition;
    private Vector3 lastMoveDir;
    public Vector3 LastMoveDir => lastMoveDir;


    public PlayerMovementHandler(PlayerStateMachine stateMachine, PlayerInputHandler inputHandler, Transform transform)
    {
        this.stateMachine = stateMachine;
        this.inputHandler = inputHandler;

        this.transform = transform;
        targetFighterPosition = transform.position;

        moveSpeed = GameRules.CombatSettings.Fighter.MoveSpeed;
        moveSnappyness = GameRules.CombatSettings.Fighter.MoveSnappyness;
    }


    /// <summary>
    /// Check for movement input in current tick buffer and resolve it
    /// </summary>
    public void TickUpdateMovement()
    {
        lastMoveDir = Vector3.zero;

        // If fighter sidesteps, set fighter in standing stance and sidestepping movement state
        if (false && inputHandler.TryReadSideStep(out bool isSideStepUp))
        {
            DebugLogger.Log("SideStep " + (isSideStepUp ? "Up" : "Down"));
            stateMachine.SetStanceState(StanceState.Standing);
            stateMachine.SetMovementState(MovementState.SideStepping);
            stateMachine.Recovery += 10;
            return;
        }

        DirectionInput cdirFlag = inputHandler.GetCurrentDirection();

        // If fighter crouchesm set fighter in crouching stance and idle movement state
        if (cdirFlag == DirectionInput.Down)
        {
            stateMachine.SetStanceState(StanceState.Crouching);
            stateMachine.SetMovementState(MovementState.Idle);
            return;
        }

        // If fighter is not crouching or sidestepping he is moving in standing stance state
        stateMachine.SetStanceState(StanceState.Standing);

        if (cdirFlag == DirectionInput.Left)
        {
            stateMachine.SetMovementState(MovementState.Retreating);
            MovePlayer(-moveSpeed * GlobalGameData.TICK_TIME * CameraManager.Instance.GetForwardDir(transform));
        }
        else if (cdirFlag == DirectionInput.Right)
        {
            stateMachine.SetMovementState(MovementState.Pushing);
            MovePlayer(moveSpeed * GlobalGameData.TICK_TIME * CameraManager.Instance.GetForwardDir(transform));
        }
        else if (cdirFlag == DirectionInput.Neutral)
        {
            stateMachine.SetMovementState(MovementState.Idle);
        }
    }
    public void MovePlayer(Vector3 addedMovement)
    {
        lastMoveDir = addedMovement.normalized;

        targetFighterPosition += addedMovement;
    }
    public void OnUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetFighterPosition, moveSnappyness * Time.deltaTime);
    }
}