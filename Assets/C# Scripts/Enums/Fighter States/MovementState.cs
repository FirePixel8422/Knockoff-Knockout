


/// <summary>
/// State describing the movement behaviour of the fighter.
/// </summary>
public enum MovementState
{
    /// <summary>
    /// Fighter is not moving
    /// </summary>
    Idle,
    /// <summary>
    /// Fighter is walking away from opponent
    /// </summary>
    Retreating,

    /// <summary>
    /// Fighter is walking towards opponent
    /// </summary>
    Pushing,
    /// <summary>
    /// Fighter is dashing towards or away from opponent
    /// </summary>
    Dashing,
    /// <summary>
    /// Fighter is sidestepping into the foreground or background
    /// </summary>
    SideStepping,
}