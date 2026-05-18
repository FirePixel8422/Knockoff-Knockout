


[System.Serializable]
public struct CombatSettings
{
    public ParrySettings Parry;
    public FighterSettings Fighter;

#if UNITY_EDITOR
    public void UpdateDebugData()
    {
        Parry.TotalParryDuration = Parry.Startup + Parry.Active + Parry.Recovery;
        Parry.AdvantageOnParry = Parry.HitStun - Parry.Recovery;
        Parry.AdvantageOnMiss = -Parry.Recovery;
    }
#endif
}