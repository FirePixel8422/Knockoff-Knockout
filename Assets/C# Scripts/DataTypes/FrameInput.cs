



using Unity.Mathematics;

[System.Serializable]
public struct FrameInput
{
    /// <summary>
    /// All attack buttons that are currently being pressed.
    /// </summary>
    public AttackInputFlags AttackFlags;
    /// <summary>
    /// Stick Direction. X is horizontal, Y is vertical. Values are between -1 and 1. (0,0) is neutral.
    /// </summary>
    public int2 Direction;
}