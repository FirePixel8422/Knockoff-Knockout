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
            collisionHandler.FrameTick_HurtBoxes(opponent.collisionHandler);
        }
    }
}