


[System.Serializable]
public struct StringTransition
{
    public int TargetMoveId;

    public StringTransition(int moveId)
    {
        TargetMoveId = moveId;
    }

    public override readonly int GetHashCode()
    {
        return TargetMoveId;
    }
}