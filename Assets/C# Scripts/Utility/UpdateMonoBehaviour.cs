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

    protected virtual void OnFrameTick() { }
    protected virtual void OnUpdate() { }
}