


/// <summary>
/// State describing the movement behaviour of the fighter.
/// </summary>
public enum MovementState
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
    /// Fighter is walking towards opponent
    /// </summary>
    Pushing,
    /// <summary>
    /// Fighter is walking away from opponent
    /// </summary>
    Retreating,
}