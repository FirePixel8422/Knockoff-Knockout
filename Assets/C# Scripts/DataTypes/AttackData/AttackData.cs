using UnityEngine;


[System.Serializable]
public struct AttackData
{
#if UNITY_EDITOR
    [SerializeField] private EditorAnimData animData;
    [SerializeField] private EditorAnimData overrideHurtAnimData;
#endif

    public FrameInput Input;
    public StanceState Stance;
    public AttackLevel Level;
    public AttackKnockDown KnockDown;

    [Header("Damage for hitting the move, hit/block knockback")]
    public float Damage;
    public float HitKb, BlockKb;

    public FrameData FrameData;

    public LungeData Lunge;
    public HomingData Homing;

    public int[] HurtBoxIds;

#if UNITY_EDITOR
    public EditorStringTransition[] StringTransitions;
    [EditorReadOnly] public int AttackId;
#endif

    [Header(">>>Baked Data<<<")]
    [EditorReadOnly] public StringTransition[] BakedStringTransitions;
    [EditorReadOnly] public AnimData AttackAnimData;
    [EditorReadOnly] public AnimData OverrideHurtAnimData;

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
    [Header("Attack On-Hit/Block/Whiff Info:")]
    [EditorReadOnly, SerializeField] private AdvantageInfoContainer normalAdvantages;

    public void BakeData()
    {
        int stringMoveCount = StringTransitions == null ? 0 : StringTransitions.Length;
        BakedStringTransitions = new StringTransition[stringMoveCount];

        for (int i = 0; i < stringMoveCount; i++)
        {
            AttackSO targetMove = StringTransitions[i].TargetMove;

            if (targetMove == null) continue;

            StringTransitions[i].BakeData(FrameData);

            BakedStringTransitions[i] = new StringTransition(targetMove.Value.AttackId);
        }

        AttackAnimData = new AnimData(animData.name, true, animData.blendIn, animData.blendOut);
        OverrideHurtAnimData = string.IsNullOrEmpty(overrideHurtAnimData.name) ? 
            GlobalAnimHashes.Missing : 
            new AnimData(overrideHurtAnimData.name, true, overrideHurtAnimData.blendIn, overrideHurtAnimData.blendOut);

        normalAdvantages.AttackDuration = (FrameData.Startup + FrameData.Active + FrameData.Recovery).ToString();
        normalAdvantages.OnHit = (FrameData.HitStun - FrameData.Recovery).ToString("+0;-0;0");
        normalAdvantages.OnBlock = (FrameData.BlockStun - FrameData.Recovery).ToString("+0;-0;0");
        normalAdvantages.OnCounter = (FrameData.CounterStun - FrameData.Recovery).ToString("+0;-0;0");
        normalAdvantages.OnWhiff = (-FrameData.Recovery).ToString("+0;-0;0");
    }
#endif
}