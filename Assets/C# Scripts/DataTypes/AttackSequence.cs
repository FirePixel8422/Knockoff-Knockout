


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
    /// End the sequence instantly.
    /// </summary>
    public void Interrupt()
    {
        State = CombatState.Idle;
        NextStateDelay = 0;
    }
}