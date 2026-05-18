


[System.Serializable]
public struct LungeData
{
    public NativeSampledAnimationCurve Curve;

    public void BakeAllCurves() => Curve.Bake();
    public readonly void Dispose() => Curve.DisposeIfCreated();
}