using UnityEngine;


[System.Serializable]
public struct ParrySettings
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