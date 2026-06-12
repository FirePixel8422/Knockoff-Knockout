

/// <summary>
/// State describing the stance of the fighter, standing or crouched
/// </summary>
public enum StanceState : byte
{
    /// <summary>
    /// Fighter is standing
    /// </summary>
    Standing,
    /// <summary>
    /// Fighter is crouching
    /// </summary>
    Crouching,

    /// <summary>
    /// Fighter is laying on the ground on their back
    /// </summary>
    KnockedDownBack,
    /// <summary>
    /// Fighter is laying on the ground on their stomach
    /// </summary>
    KnockedDownStomach,
}