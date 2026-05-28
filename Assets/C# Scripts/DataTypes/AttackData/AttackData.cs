using UnityEngine;


[System.Serializable]
public struct AttackData
{
#if UNITY_EDITOR
    public string animationName;
    public int blendIn;
    public int blendOut;
#endif
    [EditorReadOnly] public AnimData GeneratedAnimData;

    public FrameInput Input;
    [Header("In what stance is the fighter during the attack and is it a low, mid or high")]
    public StanceState Stance;
    public AttackLevel Level;

    [Header("Damage for hitting the move, hit/block knockback and does knockdown?")]
    public float Damage;
    public float HitKb, BlockKb;
    public bool DoesKnockdown;

    [Header("Lunge distance in meters and startup frame index")]
    public LungeData Lunge;

    public int[] HurtBoxIds;

    public FrameData FrameData;
    public StringTransition[] StringTransitions;

    public void BakeAllCurves()
    {
        Lunge.BakeAllCurves();
    }
    public readonly void Dispose()
    {
        Lunge.Dispose();
    }


#if UNITY_EDITOR
    [Header("Usefull Info")]
    [EditorReadOnly] public string TotalAttackDuration;
    [EditorReadOnly] public string AdvantageOnHit;
    [EditorReadOnly] public string AdvantageOnBlock;
    [EditorReadOnly] public string AdvantageOnCounter;

    public void UpdateDebugData()
    {
        GeneratedAnimData = new AnimData(animationName, true, blendIn, blendOut);

        TotalAttackDuration = (FrameData.Startup + FrameData.Active + FrameData.Recovery).ToString();
        AdvantageOnHit = (FrameData.HitStun - FrameData.Recovery).ToString("+0;-0;0");
        AdvantageOnBlock = (FrameData.BlockStun - FrameData.Recovery).ToString("+0;-0;0");
        AdvantageOnCounter = (FrameData.CounterStun - FrameData.Recovery).ToString("+0;-0;0");
    }
#endif
}