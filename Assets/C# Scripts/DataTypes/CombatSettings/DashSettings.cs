


[System.Serializable]
public struct DashSettings
{
    public int Duration;

    public int Startup;
    public int Recovery;

    public NativeSampledAnimationCurve Curve;
    public float BackDashPower;
    public float ForwardDashPower;

    public void BakeAllCurves() => Curve.Bake();
    public readonly void Dispose() => Curve.DisposeIfCreated();
}