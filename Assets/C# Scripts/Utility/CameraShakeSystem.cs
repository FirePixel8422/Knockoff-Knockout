using Fire_Pixel.Utility;
using System.Collections.Generic;
using UnityEngine;

public static class CameraShakeSystem
{
    private static readonly Queue<CameraShakeSettings> sequence = new Queue<CameraShakeSettings>();

#pragma warning disable UDR0001
    private static Transform cameraTransform;
    private static Vector3 originalLocalPos;

    private static float strength;
    private static float duration;
    private static float time;

    private static bool running;
#pragma warning restore UDR0001

    public static void PlaySequence(Camera targetCamera, params CameraShakeSettings[] steps)
    {
        if (running) return;

        cameraTransform = targetCamera.transform;
        originalLocalPos = cameraTransform.localPosition;

        sequence.Clear();

        int length = steps.Length;
        for (int i = 0; i < length; i++)
        {
            sequence.Enqueue(steps[i]);
        }

        StartNextStep();

        running = true;
        CallbackScheduler.RegisterUpdate(Update);
    }

    private static void StartNextStep()
    {
        if (sequence.Count == 0)
        {
            Stop();
            return;
        }

        CameraShakeSettings step = sequence.Dequeue();

        strength = step.Strength;
        duration = step.Duration;
        time = 0f;
    }

    private static void Update()
    {
        if (cameraTransform == null)
        {
            Stop();
            return;
        }

        time += Time.deltaTime;

        float t = time / duration;
        float currentStrength = strength * (1f - t);

        Vector3 offset = Random.insideUnitSphere * currentStrength;

        cameraTransform.localPosition = originalLocalPos + offset;

        if (time >= duration)
        {
            StartNextStep();
        }
    }

    private static void Stop()
    {
        running = false;
        CallbackScheduler.UnRegisterUpdate(Update);

        if (cameraTransform != null)
        {
            cameraTransform.localPosition = originalLocalPos;
        }
    }
}


[System.Serializable]
public struct CameraShakeSettings
{
    public float Strength;
    public float Duration;
}
