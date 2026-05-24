

/// <summary>
/// State describing the movement behaviour of the fighter.
/// </summary>
public enum MovementState : byte
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
    /// Fighter is dashing away from opponent
    /// </summary>
    DashingBack,
    /// <summary>
    /// Fighter is dashing towards opponent
    /// </summary>
    DashingForward,

    /// <summary>
    /// Fighter is sidestepping into the foreground
    /// </summary>
    SideSteppingDown,
    /// <summary>
    /// Fighter is sidestepping into the background
    /// </summary>
    SideSteppingUp,

    /// <summary>
    /// Fighter is recovering from a moveAction (Dash/Sidestep)
    /// </summary>
    Recovery,
}