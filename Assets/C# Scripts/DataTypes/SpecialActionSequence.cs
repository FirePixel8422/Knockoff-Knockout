using Unity.Mathematics;


/// <summary>
/// Datatype containing the current state of an attack and providing interaction with it with 
/// </summary>
[System.Serializable]
public struct SpecialActionSequence<TEnum>
{
    private TEnum state;
    private int nextstateDelay;

    private readonly TEnum startState, activeState, recoveryState, endState;
    private readonly int activeDuration, recoveryDuration;


    public SpecialActionSequence(
        TEnum startState, int startDuration,
        TEnum activeState, int activeDuration,
        TEnum recoveryState, int recoveryDuration,
        TEnum endState)
    {
        state = startState;
        nextstateDelay = startDuration;

        this.startState = startState;
        this.activeState = activeState;
        this.recoveryState = recoveryState;
        this.endState = endState;

        this.activeDuration = activeDuration;
        this.recoveryDuration = recoveryDuration;
    }

    /// <summary>
    /// Tick down and update State. (PostTickUpdate)
    /// </summary>
    /// <returns>True on state change. Then outputs <paramref name="newstate"/></returns>
    public bool TickUpdatestate(out TEnum newstate)
    {
        nextstateDelay -= 1;

        if (nextstateDelay > 0)
        {
            newstate = default;
            return false;
        }

        if (state.Equals(startState))
        {
            state = activeState;
            nextstateDelay = activeDuration;
        }
        else if (state.Equals(activeState))
        {
            state = recoveryState;
            nextstateDelay = recoveryDuration;
        }
        else
        {
            state = endState;
            nextstateDelay = 0;
        }

        newstate = state;
        return true;
    }

    /// <returns>The amount of ticks left that the attack would have been active for</returns>
    public int EndActivestate()
    {
        int activeTicksLeft = math.max(nextstateDelay - 1, 0);
        nextstateDelay = 0;
        return activeTicksLeft;
    }

    /// <summary>
    /// Instantly set state to <see cref="recoveryState"/>.
    /// </summary>
    public void Interrupt()
    {
        state = recoveryState;
        nextstateDelay = 0;
    }
}