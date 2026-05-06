using Fire_Pixel.Utility;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Static class for setting gamepad rumble.
/// </summary>
public static class GamepadRumble
{
    /// <summary>
    /// Set controller rumble on <paramref name="pad"/> with settings from <paramref name="vibrateParams"/>.
    /// </summary>
    public static void SetRumble(Gamepad pad, GamepadRumbleParameters vibrateParams)
    {
        CoroutineRunner.Instance.StartCoroutine(RumbleSequence(pad, vibrateParams));
    }
    private static IEnumerator RumbleSequence(Gamepad pad, GamepadRumbleParameters vibrateParams)
    {
        if (pad == null) yield break;

        vibrateParams.LowFreq = Mathf.Clamp01(vibrateParams.LowFreq);
        vibrateParams.HighFreq = Mathf.Clamp01(vibrateParams.HighFreq);

        // Active phase
        pad.SetMotorSpeeds(vibrateParams.LowFreq, vibrateParams.HighFreq);
        yield return new WaitForSeconds(vibrateParams.Duration);

        // Fade phase
        float t = 0f;

        while (t < vibrateParams.FadeTime)
        {
            t += Time.unscaledDeltaTime;

            float alpha = 1f - (t / vibrateParams.FadeTime);
            if (alpha < 0f)
            {
                alpha = 0f;
            }

            pad.SetMotorSpeeds(vibrateParams.LowFreq * alpha, vibrateParams.HighFreq * alpha);
            yield return null;
        }

        pad.SetMotorSpeeds(0f, 0f);
    }
}