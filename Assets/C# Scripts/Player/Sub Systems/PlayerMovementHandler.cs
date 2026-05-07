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

    private Vector3 targetFighterPosition;


    public PlayerMovementHandler(PlayerInputHandler inputHandler, PlayerStateMachine stateMachine, Transform transform)
    {
        this.inputHandler = inputHandler;
        this.stateMachine = stateMachine;

        this.transform = transform;
        targetFighterPosition = transform.position;
    }


    /// <summary>
    /// Check for movement input in current tick buffer and resolve it
    /// </summary>
    public void TickUpdateMovement()
    {
        DirectionInput cdirFlag = inputHandler.GetCurrentDirection();

        (MovementState movementState, Vector3 addedMovement) = cdirFlag switch
        {
            DirectionInput.Left =>
                (MovementState.Retreating, -transform.right * GlobalGameData.TICK_TIME * 12),


            DirectionInput.Right =>
                (MovementState.Pushing, transform.right * GlobalGameData.TICK_TIME * 12),

            _ =>
                (MovementState.Idle, Vector3.zero),
        };
        stateMachine.SetMovementState(movementState);
    }
    public void OnUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetFighterPosition, 12 * Time.deltaTime);
    }
}