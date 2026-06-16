using System;


/// <summary>
/// Static manager class that holds callbacks and invoke methods for various sfx
/// </summary>
public static class AudioManager
{
    private static event Action HitSFX;
    private static event Action BlockSFX;
    private static event Action PunchSFX;
    private static event Action KickSFX;

    public static void PlayOnHitSFX() => HitSFX?.Invoke();
    public static void PlayOnBlockSFX() => BlockSFX?.Invoke();
    public static void PlayOnPunchSFX() => PunchSFX?.Invoke();
    public static void PlayOnKickSFX() => KickSFX?.Invoke();
}