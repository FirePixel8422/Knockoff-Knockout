

/// <summary>
/// Datatype containing the current state of a sidestep and providing interaction with it with 
/// </summary>
[System.Serializable]
public struct SideStepSequence
{
    public MovementState State;
    public int NextStateDelay;


    public SideStepSequence(int sideStepDuration)
    {
        State = MovementState.SideStepping;
        NextStateDelay = sideStepDuration;
    }

    /// <summary>
    /// Tick down and update MovementState. (PostTickUpdate)
    /// </summary>
    /// <returns>True on state change. Then outputs <paramref name="newState"/></returns>
    public bool TickUpdateState(out MovementState newState)
    {
        NextStateDelay -= 1;

        if (NextStateDelay > 0)
        {
            newState = default;
            return false;
        }

        State = MovementState.Idle;

        newState = State;
        return true;
    }

    /// <summary>
    /// Called by the <see cref="PlayerMovementHandler"/> when the current active attack hits a target.
    /// Instantly set state to <see cref="MovementState.Idle"/>.
    /// </summary>
    public void Interrupt()
    {
        State = MovementState.Idle;
        NextStateDelay = 0;
    }
}