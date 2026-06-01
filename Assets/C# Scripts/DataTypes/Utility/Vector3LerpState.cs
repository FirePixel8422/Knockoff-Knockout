using UnityEngine;
using Unity.Mathematics;


[System.Serializable]
public struct Vector3LerpState
{
    public Vector3 Current;
    public Vector3 Target;

    public Vector3LerpState(Vector3 current)
    {
        Current = current; 
        Target = current;
    }
    public Vector3LerpState(Vector3 current, Vector3 target)
    {
        Current = current;
        Target = target;
    }

    /// <summary>
    /// Lerp <see cref="current"/> to <see cref="target"/> with percentage01 <paramref name="t"/>
    /// </summary>
    public Vector3 Lerp(float t)
    {
        Current = math.lerp(Current, Target, t);
        return Current;
    }

    /// <returns>True if the distance between <see cref="Current"/> and <see cref="Target"/> is less than <paramref name="epsilon"/>.</returns>
    public readonly bool IsCompleted(float epsilon)
    {
        float epsilonSq = epsilon * epsilon;
        return math.lengthsq(Current - Target) < epsilonSq;
    }
}