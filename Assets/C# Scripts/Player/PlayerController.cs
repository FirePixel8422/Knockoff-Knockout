using UnityEngine;


/// <summary>
/// MB class that is the core of the player controller. Holds subcomponenents and handles input.
/// </summary>
public class PlayerController : UpdateMonoBehaviour
{
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

    }
}