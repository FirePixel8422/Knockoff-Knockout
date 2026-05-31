using UnityEngine;


[System.Serializable]
public struct AttackData
{
    public FrameInput Input;
    public StanceState Stance;
    public AttackLevel Level;

    [Header("Damage for hitting the move, hit/block knockback and does knockdown?")]
    public float Damage;
    public float HitKb, BlockKb;
    public bool DoesKnockdown;

    public FrameData FrameData;

    public LungeData Lunge;
    public HomingData Homing;

    public int[] HurtBoxIds;

    public StringTransition[] StringTransitions;

    [EditorReadOnly] public AnimData GeneratedAnimData;

    public void BakeAllCurves()
    {
        if (Lunge.Power != 0)
        {
            Lunge.BakeAllCurves();
        }
        if (Homing.TotalHoming != 0)
        {
            Homing.BakeAllCurves();
        }
    }
    public readonly void Dispose()
    {
        Lunge.Dispose();
        Homing.Dispose();
    }


#if UNITY_EDITOR
    [Header("Usefull Info")]
    [EditorReadOnly] public string TotalAttackDuration;
    [EditorReadOnly] public string AdvantageOnHit;
    [EditorReadOnly] public string AdvantageOnBlock;
    [EditorReadOnly] public string AdvantageOnCounter;
    [EditorReadOnly] public string AdvantageOnWhiff;

    public void UpdateDebugData(string animName, int blendIn, int blendOut)
    {
        GeneratedAnimData = new AnimData(animName, true, blendIn, blendOut);

        TotalAttackDuration = (FrameData.Startup + FrameData.Active + FrameData.Recovery).ToString();
        AdvantageOnHit = (FrameData.HitStun - FrameData.Recovery).ToString("+0;-0;0");
        AdvantageOnBlock = (FrameData.BlockStun - FrameData.Recovery).ToString("+0;-0;0");
        AdvantageOnCounter = (FrameData.CounterStun - FrameData.Recovery).ToString("+0;-0;0");
        AdvantageOnWhiff = (-FrameData.Recovery).ToString("+0;-0;0");
    }
#endif
}