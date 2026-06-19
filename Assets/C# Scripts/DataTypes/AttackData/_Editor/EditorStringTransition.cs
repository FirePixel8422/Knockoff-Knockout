using UnityEngine;


[System.Serializable]
public struct EditorStringTransition
{
    public AttackSO TargetMove;

#if UNITY_EDITOR
    [Header("Cancel Transition On-Hit/Block/Whiff Info:")]
    [EditorReadOnly, SerializeField] private AdvantageInfoContainer cancelAdvantages;

    public void BakeData(FrameData frameData)
    {
        cancelAdvantages.AttackDuration = (frameData.Startup + frameData.Active + frameData.CancelWindow).ToString();
        cancelAdvantages.OnHit = (frameData.HitStun - frameData.CancelWindow).ToString("+0;-0;0");
        cancelAdvantages.OnBlock = (frameData.BlockStun - frameData.CancelWindow).ToString("+0;-0;0");
        cancelAdvantages.OnCounter = (frameData.CounterStun - frameData.CancelWindow).ToString("+0;-0;0");
        cancelAdvantages.OnWhiff = (-frameData.CancelWindow).ToString("+0;-0;0");
    }
#endif
}