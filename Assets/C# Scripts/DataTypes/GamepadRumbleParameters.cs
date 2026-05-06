using UnityEngine;



/// <summary>
/// Container storing gamepad rumble data
/// </summary>
[System.Serializable]
public struct GamepadRumbleParameters
{
    [Range(0, 3)]
    public float Duration;
    [Range(0, 1)]
    public float LowFreq, HighFreq;
    [Range(0, 1)]
    public float FadeTime;

    public static GamepadRumbleParameters ShortSoftRumble => new GamepadRumbleParameters
    {
        Duration = 0.25f,
        LowFreq = 0.5f,
        HighFreq = 0.5f,
        FadeTime = 0.15f
    };
}