using UnityEngine;


[System.Serializable]
public struct AttackData
{
#if UNITY_EDITOR
    public string Name;
#endif

    public float Damage;
    public float Knockback;

    public AttackType Type;
    public FrameData FrameData;

    public AttackSO Combo;
}

[System.Serializable]
public struct FrameData
{
    [Header("Move Startup, Duration, and Recovery")]
    public int Startup;
    public int ActiveFrames;
    public int NormalRecovery;
    public int CancelRecovery;

    [Header("Move OnHit and OnBlock Stun")]
    public int HitStun;
    public int BlockStun;
    public int CounterHitBonus;

    [Header("Move Hitstop for dramatic effect")]
    public int HitStop;
}