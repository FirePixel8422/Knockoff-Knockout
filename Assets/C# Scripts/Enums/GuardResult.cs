



/// <summary>
/// Result of the defender when hit by a move (attack)
/// </summary>
public enum GuardResult : byte
{
    Hit,
    Interrupted,

    StandingBlocked,
    LowBlocked,

    HighParried,
    LowParried,
}