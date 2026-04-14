using UnityEngine;
using Fire_Pixel.Utility;


public class UpdateMonoBehaviour : MonoBehaviour
{
    protected virtual void OnEnable() => CallbackScheduler.RegisterUpdate(OnUpdate);
    protected virtual void OnDisable() => CallbackScheduler.UnRegisterUpdate(OnUpdate);

    protected virtual void OnUpdate()
    {

    }
}