using System;


[Flags]
public enum AttackInputFlags : byte
{
    /// <summary>Square (Left Punch)</summary>
    B1 = 1 << 0,
    /// <summary>Triangle (Right Punch)</summary>
    B2 = 1 << 1,
    /// <summary>Cross (Left Kick)</summary>
    B3 = 1 << 2,
    /// <summary>Circle (Right Kick)</summary>
    B4 = 1 << 3,

    /// <summary>Low Parry</summary>
    B5 = 1 << 6,
    /// <summary>High Parry</summary>
    B6 = 1 << 7,

    /// <summary>L1 (Assist / Macro)</summary>
    B7 = 1 << 4,
    /// <summary>R1 (Assist / Macro)</summary>
    B8 = 1 << 5,
}