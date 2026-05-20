


[System.Serializable]
public struct CombatSettings
{
    public FighterSettings Fighter;
    public ParrySettings Parry;
    public SideStepSettings SideStep;
    public DashSettings Dash;

    public void BakeAllCurves()
    {
        SideStep.BakeAllCurves();
        Dash.BakeAllCurves();
    }
    public void Dispose()
    {
        SideStep.Dispose();
        Dash.Dispose();
    }


#if UNITY_EDITOR
    public void UpdateDebugData()
    {
        Parry.TotalParryDuration = Parry.Startup + Parry.Active + Parry.Recovery;
        Parry.AdvantageOnParry = Parry.HitStun - Parry.Recovery;
        Parry.AdvantageOnMiss = -Parry.Recovery;
    }
#endif
}