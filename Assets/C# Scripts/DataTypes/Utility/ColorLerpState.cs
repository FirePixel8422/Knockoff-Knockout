using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public struct ColorLerpState
{
    public Color Current;
    public Color Target;


    public ColorLerpState(Color current)
    {
        Current = current; 
        Target = current;
    }
    public ColorLerpState(Color current, Color target)
    {
        Current = current;
        Target = target;
    }

    /// <summary>
    /// Lerp <see cref="current"/> to <see cref="target"/> with percentage01 <paramref name="t"/>
    /// </summary>
    public Color Lerp(float t)
    {
        Current = Color.Lerp(Current, Target, t);

        return Current;
    }

    /// <returns>True if the distance between <see cref="Current"/> and <see cref="Target"/> is less than <paramref name="epsilon"/>.</returns>
    public readonly bool IsCompleted(float epsilon)
    {
        float diff =
            math.distance(Current.r, Target.r) +
            math.distance(Current.g, Target.g) +
            math.distance(Current.b, Target.b) +
            math.distance(Current.a, Target.a);

        return diff < epsilon;
    }
}