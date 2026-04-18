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
}