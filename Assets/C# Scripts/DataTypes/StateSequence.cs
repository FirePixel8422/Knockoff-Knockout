



public struct StateSequence<TEnum>
{
    public (TEnum State, int Duration)[] Sequence;


    public StateSequence(params (TEnum State, int Duration)[] sequence)
    {
        Sequence = sequence;
    }

    public readonly (TEnum State, int Duration) this[int index]
    {
        get => Sequence[index];
        set => Sequence[index] = value;
    }
    public readonly int Length => Sequence.Length;
}