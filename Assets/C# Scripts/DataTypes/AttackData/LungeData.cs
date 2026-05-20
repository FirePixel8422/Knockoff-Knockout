


[System.Serializable]
public struct LungeData
{
    public NativeSampledAnimationCurve Curve;
    public float Power;

    public void BakeAllCurves() => Curve.Bake();
    public readonly void Dispose() => Curve.DisposeIfCreated();
}