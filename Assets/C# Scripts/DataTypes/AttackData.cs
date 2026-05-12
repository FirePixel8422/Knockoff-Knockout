using UnityEngine;


[System.Serializable]
public struct AttackData
{
#if UNITY_EDITOR
    public string AnimationName;
#endif
    [EditorReadOnly] public int GeneratedAnimHash;

    public float Lunge;
    public float Damage;
    public float Knockback;

    [Header("Is the attack a low, mid or high and what hitboxes does it use")]
    public AttackLevel Level;
    public int[] HurtBoxIds;

    public FrameData FrameData;
    public FrameInput Input;

    public StringTransitions[] StringTransitions;

#if UNITY_EDITOR
    [Header("Usefull Info")]
    [EditorReadOnly] public int TotalAttackDuration;
    [EditorReadOnly] public int AdvantageOnHit;
    [EditorReadOnly] public int AdvantageOnBlock;
    [EditorReadOnly] public int AdvantageOnCounter;

    public void UpdateDebugData()
    {
        TotalAttackDuration = FrameData.Startup + FrameData.Active + FrameData.Recovery;
        AdvantageOnHit = FrameData.HitStun - FrameData.Recovery;
        AdvantageOnBlock = FrameData.BlockStun - FrameData.Recovery;
        AdvantageOnCounter = FrameData.CounterHitStun - FrameData.Recovery;
    }
#endif
}

[System.Serializable]
public struct Lunge
{
    public float Distance;
    public int StartWindow;
}

[System.Serializable]
public struct FrameData
{
    [Header("Move (Attack) Startup, Duration, Recovery and Cancel Window")]
    public int Startup;
    public int Active;
    public int Recovery;
    public int CancelWindow;

    [Header("Move (Attack) 'OnHit', 'OnBlock' and 'OnCounter' Stun")]
    public int HitStun;
    public int BlockStun;
    public int CounterHitStun;

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