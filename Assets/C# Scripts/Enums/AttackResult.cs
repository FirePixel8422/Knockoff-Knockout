

/// <summary>
/// Result of the defender when hit by a move (attack)
/// </summary>
public enum AttackResult : byte
{
    Missed,

    Hit,
    CounterHit,
    KnockDown,

    StandingBlocked,
    LowBlocked,

    Parried,
}