



public enum FighterState : byte
{
    /// <summary>
    /// Fighter is standing still upright
    /// </summary>
    Idle,
    /// <summary>
    /// Fighter is walking away from opponent
    /// </summary>
    Retreating,
    /// <summary>
    /// Fighter is crouching
    /// </summary>
    Crouching,

    /// <summary>
    /// Fighter is walking towards opponent
    /// </summary>
    Pushing,
    /// <summary>
    /// Fighter is in the middle of a Dash animation
    /// </summary>
    Dashing,
    /// <summary>
    /// Fighter is in the middle of a SideStep animation
    /// </summary>
    SideStepping,

    /// <summary>
    /// Fighter is in the startup frames of an attack animation
    /// </summary>
    MoveStartup,
    /// <summary>
    /// Fighter is in the active frames of an attack animation
    /// </summary>
    MoveActive,

    /// <summary>
    /// Fighter is in the startup frames of a Low Parry animation
    /// </summary>
    LowParryStartup,
    /// <summary>
    /// Fighter is in the active frames of a Low Parry animation
    /// </summary>
    LowParryActive,
    /// <summary>
    /// Fighter is in the startup frames of a High Parry animation
    /// </summary>
    HighParryStartup,
    /// <summary>
    /// Fighter is in the active frames of a High Parry animation
    /// </summary>
    HighParryActive,

    /// <summary>
    /// Fighter is in the recovery frame phase from executng his own attack
    /// </summary>
    Recovery,
    /// <summary>
    /// Fighter is in stun frame phase because he was hit by an attack
    /// </summary>
    HitStun,

    /// <summary>
    /// Fighter is stunned in standing block stance because it blocked a Mid/High attack
    /// </summary>
    StandingBlockStun,
    /// <summary>
    /// Fighter is stunned in crouching block stance because it blocked a Low attack
    /// </summary>
    CrouchedBlockStun,
}