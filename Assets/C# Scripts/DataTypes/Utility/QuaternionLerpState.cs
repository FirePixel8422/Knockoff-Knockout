using UnityEngine;
using Unity.Mathematics;


[System.Serializable]
public struct QuaternionLerpState
{
    public Quaternion Current;
    public Quaternion Target;

    public QuaternionLerpState(Quaternion current)
    {
        Current = current;
        Target = current;
    }
    public QuaternionLerpState(Quaternion current, Quaternion target)
    {
        Current = current;
        Target = target;
    }

    /// <summary>
    /// Lerp <see cref="current"/> to <see cref="target"/> with percentage01 <paramref name="t"/>
    /// </summary>
    public Quaternion Lerp(float t)
    {
        Current = Quaternion.Lerp(Current, Target, t);
        return Current;
    }
    /// <summary>
    /// Slerp <see cref="current"/> to <see cref="target"/> with percentage01 <paramref name="t"/>
    /// </summary>
    public Quaternion Slerp(float t)
    {
        Current = Quaternion.Slerp(Current, Target, t);
        return Current;
    }

    /// <returns>True if the distance between <see cref="Current"/> and <see cref="Target"/> is less than <paramref name="epsilon"/>.</returns>
    public readonly bool IsCompleted(float epsilon)
    {
        float dot = math.dot(Current, Target);
        return dot > (1.0f - epsilon);
    }
}