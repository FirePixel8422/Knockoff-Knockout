using UnityEngine;



[System.Serializable]
public struct CombatSettings
{
    public Parry Parry;

#if UNITY_EDITOR
    public void UpdateDebugData()
    {
        Parry.TotalParryDuration = Parry.Startup + Parry.Active + Parry.Recovery;
        Parry.AdvantageOnParry = Parry.HitStun - Parry.Recovery;
        Parry.AdvantageOnMiss = -Parry.Recovery;
    }
#endif
}

[System.Serializable]
public struct Parry
{
    [Header("(Parry) Startup, Duration and Recovery")]
    public int Startup;
    public int Active;
    public int Recovery;

    [Header("(Parry) 'OnParry' stun")]
    public int HitStun;

    [Header("(Parry) HitStop for dramatic effect 'OnParry'")]
    public int HitStop;

#if UNITY_EDITOR
    [Header("Usefull Info")]
    [EditorReadOnly] public int TotalParryDuration;
    [EditorReadOnly] public int AdvantageOnParry;
    [EditorReadOnly] public int AdvantageOnMiss;
#endif
}