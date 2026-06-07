


[System.Serializable]
public struct SideStepSettings
{
    public int Duration;
    public int Startup;
    public int Recovery;

    public NativeSampledAnimationCurve DurationCurve;

    public float SideStepPower;
    public float ForwardPower;
    public MinMaxFloat DistanceRange;

    public void BakeAllCurves()
    {
        DurationCurve.Bake();
    }
    public readonly void Dispose()
    {
        DurationCurve.DisposeIfCreated();
    }
}