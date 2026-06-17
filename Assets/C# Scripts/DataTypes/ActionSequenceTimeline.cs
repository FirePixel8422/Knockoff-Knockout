using Unity.Mathematics;


/// <summary>
/// Class datatype containing the current state of an action and providing interaction with it with
/// </summary>
[System.Serializable]
public struct ActionSequenceTimeline<TEnum>
{
    private readonly StateSequence<TEnum> stateSequence;

    private int currentStateId;
    private int nextStateDelay;

    public int ElapsedTicks;
    public readonly bool IsActive => currentStateId != stateSequence.Length - 1;
    public readonly TEnum CurrentState => stateSequence[currentStateId].State;


    public ActionSequenceTimeline(StateSequence<TEnum> newSequence)
    {
        stateSequence = newSequence;

        currentStateId = 0;
        nextStateDelay = stateSequence[0].Duration;
        ElapsedTicks = 0;
    }

    /// <summary>
    /// Tick down and update action state. (PostTickUpdate)
    /// </summary>
    /// <returns>True on state change. Then outputs <paramref name="newState"/></returns>
    public bool TickUpdateState(out TEnum newState, out int elapsedTicks)
    {
        nextStateDelay -= 1;
        ElapsedTicks += 1;

        if (nextStateDelay > 0)
        {
            newState = default;
            elapsedTicks = ElapsedTicks;
            return false;
        }

        currentStateId += 1;
        nextStateDelay = stateSequence[currentStateId].Duration;

        newState = stateSequence[currentStateId].State;
        elapsedTicks = ElapsedTicks;
        return true;
    }

    /// <summary>
    /// Instantly mark sequence for advancement to next state.
    /// </summary>
    /// <returns>The amount of ticks left in the current state</returns>
    public int AdvanceState()
    {
        int activeTicksLeft = math.max(nextStateDelay - 1, 0);
        nextStateDelay = 0;
        return activeTicksLeft;
    }

    /// <summary>
    /// Instantly set state to end state
    /// </summary>
    public void Cancel()
    {
        currentStateId = stateSequence.Length - 1;
        nextStateDelay = 0;
    }
}