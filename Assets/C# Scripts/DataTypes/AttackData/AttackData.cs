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
    [SerializeField] private EditorStringTransition[] stringTransitions;
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
    [EditorReadOnly, SerializeField] private AdvantageContainer normalAdvantages;

    public void BakeData()
    {
        int stringMoveCount = stringTransitions == null ? 0 : stringTransitions.Length;
        BakedStringTransitions = new StringTransition[stringMoveCount];

        for (int i = 0; i < stringMoveCount; i++)
        {
            if (stringTransitions[i].TargetMove == null) continue;

            stringTransitions[i].BakeData(FrameData);

            BakedStringTransitions[i] = new StringTransition(
                stringTransitions[i].TargetMove.Value.AttackAnimData.Hash);
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

    [System.Serializable]
    public struct EditorStringTransition
    {
        public AttackSO TargetMove;

        [Header("Cancel Transition On-Hit/Block/Whiff Info:")]
        [EditorReadOnly, SerializeField] private AdvantageContainer cancelAdvantages;

        public void BakeData(FrameData frameData)
        {
            cancelAdvantages.AttackDuration = (frameData.Startup + frameData.Active + frameData.CancelWindow).ToString();
            cancelAdvantages.OnHit = (frameData.HitStun - frameData.CancelWindow).ToString("+0;-0;0");
            cancelAdvantages.OnBlock = (frameData.BlockStun - frameData.CancelWindow).ToString("+0;-0;0");
            cancelAdvantages.OnCounter = (frameData.CounterStun - frameData.CancelWindow).ToString("+0;-0;0");
            cancelAdvantages.OnWhiff = (-frameData.CancelWindow).ToString("+0;-0;0");
        }
    }
    [System.Serializable]
    public struct AdvantageContainer
    {
        [EditorReadOnly] public string AttackDuration;
        [EditorReadOnly] public string OnHit;
        [EditorReadOnly] public string OnBlock;
        [EditorReadOnly] public string OnCounter;
        [EditorReadOnly] public string OnWhiff;
    }
    [System.Serializable]
    public struct EditorAnimData
    {
        public string name;
        public int blendIn;
        public int blendOut;
    }
#endif
}