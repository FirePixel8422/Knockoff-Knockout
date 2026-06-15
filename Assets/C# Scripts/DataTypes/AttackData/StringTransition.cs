


[System.Serializable]
public struct StringTransition
{
    public int TargetMoveHash;

    public StringTransition(int moveHash)
    {
        TargetMoveHash = moveHash;
    }

    public override readonly int GetHashCode()
    {
        return TargetMoveHash;
    }
}