using Unity.Mathematics;



[System.Serializable]
public struct HomingData
{
    public NativeSampledAnimationCurve Curve;
    public int2 Window;
    public float TotalHoming;

    public void BakeAllCurves() => Curve.Bake();
    public readonly void Dispose() => Curve.DisposeIfCreated();
}