using Unity.Mathematics;



[System.Serializable]
public struct FloatLerpState
{
    public float Current;
    public float Target;

    public FloatLerpState(float current, float target)
    {
        Current = current; 
        Target = target;
    }

    /// <summary>
    /// Lerp <see cref="current"/> to <see cref="target"/> with percentage01 <paramref name="t"/>
    /// </summary>
    public float Lerp(float t)
    {
        Current = math.lerp(Current, Target, t);
        return Current;
    }

    /// <returns>True if the distance between <see cref="current"/> and <see cref="target"/> is less then <paramref name="epsilon"/></returns>
    public readonly bool IsCompleted(float epsilon)
    {
        return math.abs(Current - Target) < epsilon;
    }
}