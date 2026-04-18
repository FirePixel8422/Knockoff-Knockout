using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public static class GamepadRumble
{
    public static IEnumerator Rumble(Gamepad pad, GamepadRumbleParameters vibrateParams)
    {
        if (pad == null)
        {
            yield break;
        }

        vibrateParams.LowFreq = Mathf.Clamp01(vibrateParams.LowFreq);
        vibrateParams.HighFreq = Mathf.Clamp01(vibrateParams.HighFreq);

        // Active phase
        pad.SetMotorSpeeds(vibrateParams.LowFreq, vibrateParams.HighFreq);
        yield return new WaitForSeconds(vibrateParams.Duration);

        // Fade phase
        float t = 0f;

        while (t < vibrateParams.FadeTime)
        {
            t += Time.deltaTime;

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