using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public struct AttackData
{
#if UNITY_EDITOR
    public string AnimationName;
#endif

    public float Damage;
    public float Knockback;

    public AttackLevel Level;
    public FrameData FrameData;
    public FrameInput Input;

    public StringTransitions[] StringTransitions;

    [EditorReadOnly] public int GeneratedAnimHash;

#if UNITY_EDITOR
    [EditorReadOnly] public int TotalAttackDuration;
    [EditorReadOnly] public int AdvantageOnHit;
    [EditorReadOnly] public int AdvantageOnBlock;
#endif
}

[System.Serializable]
public struct FrameData
{
    [Header("Move (Attack) Startup, Duration, Recovery and Cancellable Window")]
    public int Startup;
    public int ActiveFrames;
    public int Recovery;
    public int2 CancelWindow;

    [Header("Move (Attack) OnHit and OnBlock Stun")]
    public int HitStun;
    public int BlockStun;
    public int CounterHitBonus;

    [Header("Move (Attack) Hitstop and BlockStop for dramatic effect when connecting it")]
    public int HitStop;
    public int BlockStop;
}

[System.Serializable]
public struct StringTransitions
{
    public AttackSO TargetMove;
    public int frameSkipCount;
}