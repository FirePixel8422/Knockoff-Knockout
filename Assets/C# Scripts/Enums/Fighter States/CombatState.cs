


/// <summary>
/// State describing the stance of the fighter.
/// </summary>
public enum CombatState
{
    /// <summary>
    /// Fighter is just chilling (Also used as null state, no state)
    /// </summary>
    Idle,

    /// <summary>
    /// Fighter is in the recovery frame phase from executng an action, such as an attack, parry, dash, sidestep, etc.
    /// </summary>
    Recovering,

    /// <summary>
    /// Fighter is in the startup frames of an attack animation
    /// </summary>
    AttackStartup,
    /// <summary>
    /// Fighter is in the active frames of an attack animation
    /// </summary>
    AttackActive,

    /// <summary>
    /// Fighter is in stun frame phase because he was hit by an attack
    /// </summary>
    HitStun,
    /// <summary>
    /// Fighter is stunned in block stance because they blocked a Low while crouching or Mid/High while standing
    /// </summary>
    BlockStun,

    /// <summary>
    /// Fighter is in the active frames of a ParryLow animation
    /// </summary>
    ParryLow,

    /// <summary>
    /// Fighter is in the active frames of a ParryHigh animation
    /// </summary>
    ParryHigh,
}