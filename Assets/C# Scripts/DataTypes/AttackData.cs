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

    [Header("Is the attack a low, mid or high and what hitboxes does it use")]
    public AttackLevel Level;
    public int[] HurtBoxIds;

    public FrameData FrameData;
    public FrameInput Input;

    public StringTransitions[] StringTransitions;

    [EditorReadOnly] public int GeneratedAnimHash;

#if UNITY_EDITOR
    [Header("Usefull Info")]
    [EditorReadOnly] public int TotalAttackDuration;
    [EditorReadOnly] public int AdvantageOnHit;
    [EditorReadOnly] public int AdvantageOnBlock;

    public void UpdateDebugData()
    {
        TotalAttackDuration = FrameData.Startup + FrameData.Active + FrameData.Recovery;
        AdvantageOnHit = FrameData.HitStun - FrameData.Recovery;
        AdvantageOnBlock = FrameData.BlockStun - FrameData.Recovery;
    }
#endif
}

[System.Serializable]
public struct FrameData
{
    [Header("Move (Attack) Startup, Duration, Recovery and Cancel Window")]
    public int Startup;
    public int Active;
    public int Recovery;
    public int2 CancelWindow;

    [Header("Move (Attack) 'OnHit' and 'OnBlock' Stun")]
    public int HitStun;
    public int BlockStun;
    public int CounterHitBonus;

    [Header("Move (Attack) HitStop and BlockStop for dramatic effect when connecting it")]
    public int HitStop;
    public int BlockStop;
}

[System.Serializable]
public struct StringTransitions
{
    public AttackSO TargetMove;
    public int frameSkipCount;
}