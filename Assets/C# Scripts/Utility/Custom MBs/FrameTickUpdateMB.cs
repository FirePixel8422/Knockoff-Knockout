using UnityEngine;
using Fire_Pixel.Utility;


public class FrameTickUpdateMB : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        CallbackScheduler.RegisterUpdate(OnUpdate);
        CallbackScheduler.RegisterTickUpdate(OnTickUpdate);
    }
    protected virtual void OnDisable()
    {
        CallbackScheduler.UnRegisterUpdate(OnUpdate);
        CallbackScheduler.UnRegisterTickUpdate(OnTickUpdate);
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    protected virtual void OnUpdate() { }

    /// <summary>
    /// Called every game tick, caught up if the game is running behind. Use for logic that needs to be executed every tick, regardless of frame rate.
    /// </summary>
    protected virtual void OnTickUpdate() { }
}