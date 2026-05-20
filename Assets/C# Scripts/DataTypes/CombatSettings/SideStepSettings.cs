


[System.Serializable]
public struct SideStepSettings
{
    public int Duration;
    public int Recovery;

    public NativeSampledAnimationCurve Curve;
    public float Rotation;

    public void BakeAllCurves() => Curve.Bake();
    public readonly void Dispose() => Curve.DisposeIfCreated();
}