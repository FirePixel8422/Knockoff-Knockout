using System;


/// <summary>
/// Static manager class that holds callbacks and invoke methods for various sfx
/// </summary>
public static class AudioManager
{
    public static event Action HitSFX;
    public static event Action CounterSFX;
    public static event Action BlockSFX;

    public static event Action HurtSFX;
    public static event Action KnockDownSFX;

    public static event Action PunchSFX;
    public static event Action KickSFX;

    public static event Action DeathSFX;


    public static void PlayHitSFX() => HitSFX?.Invoke();
    public static void PlayBlockSFX() => BlockSFX?.Invoke();
    public static void PlayPunchSFX() => PunchSFX?.Invoke();
    public static void PlayKickSFX() => KickSFX?.Invoke();
    public static void PlayKnockDownSFX() => KnockDownSFX?.Invoke();
}