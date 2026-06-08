


[System.Serializable]
public struct StringTransition
{
    public int TargetMoveHash;
    public int frameSkipCount;

    public StringTransition(int moveHash, int frameSkipCount)
    {
        this.TargetMoveHash = moveHash;
        this.frameSkipCount = frameSkipCount;
    }

    public override readonly int GetHashCode()
    {
        return TargetMoveHash;
    }
}