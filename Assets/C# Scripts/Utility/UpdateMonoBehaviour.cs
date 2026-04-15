using UnityEngine;
using Fire_Pixel.Utility;


public class UpdateMonoBehaviour : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        CallbackScheduler.RegisterUpdate(OnUpdate);
        CallbackScheduler.RegisterFrameTick(OnFrameTick);
    }
    protected virtual void OnDisable()
    {
        CallbackScheduler.UnRegisterUpdate(OnUpdate);
        CallbackScheduler.UnRegisterFrameTick(OnFrameTick);
    }

    /// <summary>
    /// Called every game tick, caught up if the game is running behind. Use for logic that needs to be executed every tick, regardless of frame rate.
    /// </summary>
    protected virtual void OnFrameTick() { }
    /// <summary>
    /// Called every frame.
    /// </summary>
    protected virtual void OnUpdate() { }
}