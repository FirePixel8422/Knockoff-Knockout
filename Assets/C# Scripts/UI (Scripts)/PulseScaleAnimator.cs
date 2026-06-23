using UnityEngine;


public class PulseScaleAnimator : UpdateMonoBehaviour
{
    [SerializeField] private float targetScaleMultiplier = 1.2f;
    [SerializeField] private float targetSpeed = 2f;

    protected override void OnUpdate()
    {
        float t = (Mathf.Sin(Time.unscaledTime * targetSpeed) + 1f) * 0.5f;
        float scale = Mathf.Lerp(1f, targetScaleMultiplier, t);

        transform.localScale = Vector3.one * scale;
    }
}