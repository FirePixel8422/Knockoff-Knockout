using System;


/// <summary>
/// Static manager class that holds callbacks and invoke methods for various sfx
/// </summary>
public static class AudioManager
{
    public static event Action OnHitSFX;
    public static event Action OnBlockSFX;
    public static event Action OnPunchSFX;
    public static event Action OnKickSFX;

    public static void PlayOnHit() => OnHitSFX?.Invoke();
    public static void PlayOnBlock() => OnBlockSFX?.Invoke();
    public static void PlayOnPunch() => OnPunchSFX?.Invoke();
    public static void PlayOnKick() => OnKickSFX?.Invoke();
}