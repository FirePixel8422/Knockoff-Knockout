using UnityEngine;



public class PlayerStateMachine
{
    [SerializeField] private FighterState state;
    [SerializeField] private AttackData currentMove;


    public FighterState State => state;
    public AttackData CurrentMove => currentMove;
    public bool IsStunned => state == FighterState.HitStun || state == FighterState.StandingBlockStun || state == FighterState.HitStun;

    private Animator anim;


    public void Init(Transform playerRoot)
    {
        anim = playerRoot.GetComponent<Animator>();
    }

    public void UpdateState()
    {

    }
}