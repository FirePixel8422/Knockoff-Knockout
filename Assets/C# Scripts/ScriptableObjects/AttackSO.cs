using UnityEngine;



[CreateAssetMenu(fileName = "New Move", menuName = "ScriptableObjects/Combat/Move", order = -1003)]
public class AttackSO : ScriptableObject
{
    public AttackData Value;
    

    private void OnValidate()
    {
        Value.GeneratedAnimHash = Animator.StringToHash(Value.AnimationName);

        Value.AdvantageOnHit = Value.FrameData.HitStun - Value.FrameData.Recovery;
        Value.AdvantageOnBlock = Value.FrameData.BlockStun - Value.FrameData.Recovery;
        Value.TotalAttackDuration = Value.FrameData.Startup + Value.FrameData.ActiveFrames + Value.FrameData.Recovery;
    }
}