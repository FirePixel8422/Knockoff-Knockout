


/// <summary>
/// Struct containing fighter state info. <see cref="MovementState"/> and <see cref="CombatState"/>.
/// </summary>
[System.Serializable]
public struct FighterState
{
    public StanceState StanceState;
    public MovementState MovementState;
    public CombatState CombatState;


    /// <summary>
    /// Whether the fighter is able to block attacks in their current state.
    /// </summary>
    public readonly bool CanBlock()
    {
        // Defender can block if they are moving backwards or idle + they are not doing an attack or are in blockstun
        return
            (MovementState == MovementState.Idle || MovementState == MovementState.Retreating) &&
            (CombatState == CombatState.Idle || CombatState == CombatState.BlockStun);
    }
}