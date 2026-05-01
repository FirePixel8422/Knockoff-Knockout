



/// <summary>
/// Struct containing fighter state info. <see cref="MovementState"/> and <see cref="CombatState"/>.
/// </summary>
[System.Serializable]
public struct FighterState
{
    public CombatState CombatState;
    public MovementState MovementState;
}