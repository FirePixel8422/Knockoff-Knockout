using Unity.Mathematics;


/// <summary>
/// Datatype containing the current state of an attack and providing interaction with it with 
/// </summary>
[System.Serializable]
public struct AttackSequence
{
    private AttackData attack;
    public readonly AttackData Attack => attack;

    private CombatState state;
    private int nextStateDelay;
    public readonly bool IsActive => state != CombatState.Idle;


    public AttackSequence(AttackData attack)
    {
        this.attack = attack;
        state = CombatState.ActionStartup;
        nextStateDelay = attack.FrameData.Startup;
    }

    /// <summary>
    /// Tick down and update AttackState. (PostTickUpdate)
    /// </summary>
    /// <returns>True on state change. Then outputs <paramref name="newState"/></returns>
    public bool TickUpdateState(out CombatState newState)
    {
        nextStateDelay -= 1;

        if (nextStateDelay > 0)
        {
            newState = default;
            return false;
        }

        (state, nextStateDelay) = state switch
        {
            CombatState.ActionStartup =>
                (CombatState.AttackActive, Attack.FrameData.Active),

            CombatState.AttackActive =>
                (CombatState.Recovering, Attack.FrameData.Recovery),

            CombatState.Recovering or _ =>
                (CombatState.Idle, 0),
        };

        newState = state;
        return true;
    }

    /// <summary>
    /// Called by the <see cref="PlayerAttackHandler"/> when the current active attack hits a target.
    /// Instantly set state progression from <see cref="CombatState.AttackActive"/> to the <see cref="CombatState.Recovering"/>.
    /// (State will be applied in <see cref="PlayerController.PostTickUpdate"/>)
    /// </summary>
    /// <returns>The amount of ticks left that the attack would have been active for</returns>
    public int EndAttackActiveState()
    {
        int activeTicksLeft = math.max(nextStateDelay - 1, 0);
        nextStateDelay = 0;
        return activeTicksLeft;
    }

    /// <summary>
    /// Called by the <see cref="PlayerAttackHandler"/>  when the current active attack hits a target.
    /// Instantly set state to <see cref="CombatState.Idle"/>.
    /// </summary>
    public void Interrupt()
    {
        state = CombatState.Idle;
        nextStateDelay = 0;
    }
}