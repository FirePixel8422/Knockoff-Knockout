


using Unity.Mathematics;

/// <summary>
/// Datatype containing the current state of an attack and providing interaction with it with 
/// </summary>
[System.Serializable]
public struct AttackSequence
{
    public AttackData Attack;

    public CombatState State;
    public int NextStateDelay;


    public AttackSequence(AttackData attack)
    {
        Attack = attack;
        State = CombatState.AttackStartup;
        NextStateDelay = Attack.FrameData.Startup;
    }

    /// <summary>
    /// Tick down and update AttackState. (PostTickUpdate)
    /// </summary>
    /// <returns>True on state change. Then outputs <paramref name="newState"/></returns>
    public bool TickUpdateState(out CombatState newState)
    {
        NextStateDelay -= 1;

        if (NextStateDelay > 0)
        {
            newState = default;
            return false;
        }

        (State, NextStateDelay) = State switch
        {
            CombatState.AttackStartup =>
                (CombatState.AttackActive, Attack.FrameData.Active),

            CombatState.AttackActive =>
                (CombatState.Recovering, Attack.FrameData.Recovery),

            CombatState.Recovering or _ =>
                (CombatState.Idle, 0),
        };

        newState = State;
        return true;
    }

    /// <summary>
    /// Called by the <see cref="PlayerAttackHandler"/>  when the current active attack hits a target.
    /// Instantly set state progression from <see cref="CombatState.AttackActive"/> to the <see cref="CombatState.Recovering"/>.
    /// (State will be applied in <see cref="PlayerController.PostTickUpdate"/>)
    /// </summary>
    /// <returns>The amount of ticks left that the attack would have been active for</returns>
    public int EndAttackActiveState()
    {
        int activeTicksLeft = math.max(NextStateDelay - 1, 0);
        NextStateDelay = 0;
        return activeTicksLeft;
    }

    /// <summary>
    /// Called by the <see cref="PlayerAttackHandler"/>  when the current active attack hits a target.
    /// Instantly set state to <see cref="CombatState.Idle"/>.
    /// </summary>
    public void Interrupt()
    {
        State = CombatState.Idle;
        NextStateDelay = 0;
    }
}