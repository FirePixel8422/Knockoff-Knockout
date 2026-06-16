using System;


/// <summary>
/// Static manager class that holds callbacks and invoke methods for various sfx
/// </summary>
public static class AudioManager
{
    public static event Action HitSFX;
    public static event Action BlockSFX;
    public static event Action PunchSFX;
    public static event Action KickSFX;

    public static void PlayOnHitSFX() => HitSFX?.Invoke();
    public static void PlayOnBlockSFX() => BlockSFX?.Invoke();
    public static void PlayOnPunchSFX() => PunchSFX?.Invoke();
    public static void PlayOnKickSFX() => KickSFX?.Invoke();
}