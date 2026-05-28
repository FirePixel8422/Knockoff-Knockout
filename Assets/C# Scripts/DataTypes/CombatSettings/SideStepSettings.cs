


[System.Serializable]
public struct SideStepSettings
{
    public int Duration;
    public int Startup;
    public int Recovery;

    public NativeSampledAnimationCurve DurationCurve;
    public NativeSampledAnimationCurve PowerCurve;

    public float SideStepPower;
    public float ForwardPower;
    public MinMaxFloat DistanceRange;

    public void BakeAllCurves()
    {
        DurationCurve.Bake();
        PowerCurve.Bake();
    }
    public readonly void Dispose()
    {
        DurationCurve.DisposeIfCreated();
        PowerCurve.DisposeIfCreated();
    }
}