


/// <summary>
/// Datatype containing the current state of an attack and providing interaction with it with 
/// </summary>
[System.Serializable]
public struct AttackSequence
{
    public AttackData Attack;

    public AttackProgressionState State;
    public int NextStateDelay;


    public AttackSequence(AttackData attack)
    {
        Attack = attack;
        State = AttackProgressionState.Startup;
        NextStateDelay = Attack.FrameData.Startup;
    }

    /// <summary>
    /// Tick down <see cref="NextStateDelay"/> and update to next state accordingly. Returns whether sequence has finished
    /// </summary>
    public void OnFrameTick(out bool sequenceFinished)
    {
        NextStateDelay -= 1;

        if (NextStateDelay == 0)
        {
            (State, NextStateDelay) = State switch
            {
                AttackProgressionState.Startup => 
                    (AttackProgressionState.Active, Attack.FrameData.Active),

                AttackProgressionState.Active => 
                    (AttackProgressionState.Recovery, Attack.FrameData.Recovery),

                AttackProgressionState.Recovery or _ => 
                    (AttackProgressionState.Ended, 0),
            };
        }

        sequenceFinished = State == AttackProgressionState.Ended;
    }

    /// <summary>
    /// End the sequence instantly.
    /// </summary>
    public void InteruptAttack()
    {
        State = AttackProgressionState.Ended;
        NextStateDelay = 0;
    }
}