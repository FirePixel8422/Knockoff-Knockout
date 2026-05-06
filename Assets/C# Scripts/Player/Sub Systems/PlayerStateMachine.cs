using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for handling player state
/// </summary>
[System.Serializable]
public class PlayerStateMachine
{
    public FighterState State;
    [SerializeField] private FighterState bufferedState;

    public int HitStop;

    public int Recovery;
    public int HitStun;
    public int BlockStun;
    public bool IsStunned =>
        HitStop > 0 ||
        Recovery > 0 ||
        HitStun > 0 ||
        BlockStun > 0;

    private readonly Animator anim;


    public PlayerStateMachine(Transform playerRoot)
    {
        anim = playerRoot.GetComponent<Animator>();
        anim.enabled = false;
    }

    public void ResolveAttack(AttackData move, bool isTarget)
    {
        HitStop += move.FrameData.HitStop;

        if (isTarget)
        {
            HitStun += move.FrameData.HitStun;
            BlockStun += move.FrameData.BlockStun;
        }
        else
        {
            Recovery += move.FrameData.Recovery;
        }
    }

    /// <summary>
    /// Update animator and tick down stun states
    /// </summary>
    public void TickUpdate()
    {
        if (HitStop > 0)
        {
            anim.speed = 0;
            HitStop -= 1;
            return;
        }

        bool wasStunned = IsStunned;
        Recovery = Mathf.Clamp(Recovery - 1, 0, int.MaxValue);
        HitStun = Mathf.Clamp(HitStun - 1, 0, int.MaxValue);
        BlockStun = Mathf.Clamp(BlockStun - 1, 0, int.MaxValue);

        // If player was stunned and just recovered, set state to buffered state
        if (wasStunned && (IsStunned == false))
        {
            SetFighterState(bufferedState);
        }

        anim.speed = 1;
        anim.Update(GlobalGameData.TICK_TIME);
    }
    public void SetFighterState(FighterState newState)
    {
        State = newState;
    }
    public void PlayAnimation(int animHash, int transitionFrames = 0, int layer = 0)
    {
        if (transitionFrames == 0)
        {
            anim.Play(animHash, layer);
        }
        else
        {
            anim.CrossFade(animHash, transitionFrames * GlobalGameData.TICK_TIME, layer);
        }
    }
}