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
    private MovementState movementState
    {
        get => stateMachine.State.MovementState;
        set => stateMachine.State.MovementState = value;
    }

    private Vector3 targetFighterPosition;


    public PlayerMovementHandler(PlayerInputHandler inputHandler, PlayerStateMachine stateMachine, Transform transform)
    {
        this.inputHandler = inputHandler;
        this.stateMachine = stateMachine;

        this.transform = transform;
        targetFighterPosition = transform.position;
    }


    public void OnFrameTick()
    {
        DirectionInput cdirFlag = inputHandler.GetCurrentDirection();

        switch (cdirFlag)
        {
            case DirectionInput.Left:
                targetFighterPosition -= transform.right * GlobalGameData.TICK_TIME * 12;
                movementState = MovementState.Retreating;
                break;


            case DirectionInput.Right:
                targetFighterPosition += transform.right * GlobalGameData.TICK_TIME * 12;
                movementState = MovementState.Pushing;
                break;

            default:
                movementState = MovementState.Idle;
                break;
        }
        ;
    }
    public void OnUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetFighterPosition, 12 * Time.deltaTime);
    }
}