using UnityEngine;


/// <summary>
/// MB class that is the core of the player controller. Holds subcomponenents and handles input.
/// </summary>
public class PlayerController : UpdateMonoBehaviour
{
    [SerializeField] private PlayerController opponent;
    [SerializeField] private PlayerColliderHandler collisionHandler;

    public FighterState State;
    public bool IsAssigned;


    private void Awake()
    {
        collisionHandler.Init(transform);
    }

    public void OnButton1(bool performed)
    {
        //DebugLogger.Log(gameObject.name + "Square" + performed);
    }
    public void OnButton2(bool performed)
    {
        //DebugLogger.Log(gameObject.name + "Triangle" + performed);
    }
    public void OnButton3(bool performed)
    {
        //DebugLogger.Log(gameObject.name + "Cross" + performed);
    }



    protected override void OnFrameTick()
    {
        if (State == FighterState.MoveActive)
        {
            bool hit = CollisionUtils.CheckAABBIntersection(opponent.collisionHandler.HitBoxes, collisionHandler.HurtBoxes);

            print(hit);
        }

        bool isStunned = State == FighterState.HitStun || State == FighterState.StandingBlockStun || State == FighterState.HitStun;
    }
}