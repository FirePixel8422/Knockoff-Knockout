


[System.Serializable]
public struct DashSettings
{
    public int Duration;
    public int Recovery;

    public NativeSampledAnimationCurve Curve;
    public float Power;

    public void BakeAllCurves() => Curve.Bake();
    public readonly void Dispose() => Curve.DisposeIfCreated();
}