using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for performinmg and tracking attack states.
/// </summary>
[System.Serializable]
public class PlayerAttackHandler
{
    private readonly PlayerInputHandler inputHandler;
    private readonly PlayerStateMachine stateMachine;

    [SerializeField] private AttackSequence currentSequence;
    [SerializeField] private bool sequenceActive;
    public AttackData CurrentActiveAttack => currentSequence.Attack;


    public PlayerAttackHandler(PlayerInputHandler inputHandler, PlayerStateMachine stateMachine)
    {
        this.inputHandler = inputHandler;
        this.stateMachine = stateMachine;
    }


    public void OnFrameTick()
    {
        if (sequenceActive)
        {
            currentSequence.OnFrameTick(out bool sequenceFinished);
            if (sequenceFinished)
            {
                TryCreateNewAttackSequence();
                sequenceActive = false;
            }
            return;
        }

        TryCreateNewAttackSequence();
    }

    private void TryCreateNewAttackSequence()
    {
        if (inputHandler.TryReadAttack(out AttackData targetAttack))
        {
            currentSequence = new AttackSequence(targetAttack);
        }
    }


    // Mental/Logic Notes:
    //
    // Inputs are always between ticks, either before or after, NEVER during. (This is because inputs get collected in Update and Update runs before and sepperately from Tick)
    //
    // Crouching (State) transitions to sidestep (State) when the down input gets pressed + released within 1-3ish frames.
    // When sidestep is inputted, animation cancels from wherever crouch was into sidestep INSTANTLY and the fighter is now INSTANTLY considered standing
    //
    // When fighter presses down before next tick, they are considered crouched in that next tick.
    // And so when fighter releases down before next tick, they are considered standing in that next tick.
    //
    // When an attack is inputted, on the next frame it gets executed, that tick counts towards the attacks duration (That tick is the first tick of the attack startup).
}