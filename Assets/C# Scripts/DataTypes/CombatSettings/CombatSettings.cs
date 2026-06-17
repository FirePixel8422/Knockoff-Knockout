


[System.Serializable]
public struct CombatSettings
{
    public FighterSettings Fighter;
    public SideStepSettings SideStep;
    public DashSettings Dash;
    public float MaxAttackRealignmentDeg;

    public void BakeAllCurves()
    {
        SideStep.BakeAllCurves();
        Dash.BakeAllCurves();
    }
    public readonly void Dispose()
    {
        SideStep.Dispose();
        Dash.Dispose();
    }
}