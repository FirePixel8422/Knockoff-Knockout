using UnityEngine;


[System.Serializable]
public struct AttackData
{
#if UNITY_EDITOR
    public string AnimationName;
#endif
    [EditorReadOnly] public int GeneratedAnimHash;

    [Header("Damage for hitting the move, hit/block knockback and does knockdown?")]
    public float Damage;
    public float HitKB, BlockKB;
    public bool DoesKnockDown;

    [Header("Lunge distance in meters and startup frame index")]
    public Lunge Lunge;

    [Header("Is the attack a low, mid or high and what hurtboxes does it use")]
    public AttackLevel Level;
    public int[] HurtBoxIds;

    public FrameData FrameData;
    public FrameInput Input;

    public StringTransitions[] StringTransitions;

#if UNITY_EDITOR
    [Header("Usefull Info")]
    [EditorReadOnly] public string TotalAttackDuration;
    [EditorReadOnly] public string AdvantageOnHit;
    [EditorReadOnly] public string AdvantageOnBlock;
    [EditorReadOnly] public string AdvantageOnCounter;

    public void UpdateDebugData()
    {
        TotalAttackDuration = (FrameData.Startup + FrameData.Active + FrameData.Recovery).ToString();
        AdvantageOnHit = (FrameData.HitStun - FrameData.Recovery).ToString("+0;-0;0");
        AdvantageOnBlock = (FrameData.BlockStun - FrameData.Recovery).ToString("+0;-0;0");
        AdvantageOnCounter = (FrameData.CounterStun - FrameData.Recovery).ToString("+0;-0;0");
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
    public int CounterStun;

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