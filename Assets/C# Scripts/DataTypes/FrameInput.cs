



[System.Serializable]
public struct FrameInput
{
    /// <summary>
    /// All attack buttons that are currently being pressed.
    /// </summary>
    public AttackInputFlags AttackFlags;
    /// <summary>
    /// Stick Direction, ignoring diagonals
    /// </summary>
    public DirectionInputFlag DirectionFlag;
}