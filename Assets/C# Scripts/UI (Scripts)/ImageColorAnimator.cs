using UnityEngine;
using UnityEngine.UI;


public class ImageColorAnimator : UpdateMonoBehaviour
{
    [SerializeField] private Color a, b;
    [SerializeField] private float lerpTime;

    private Image targetImage;
    private float elapsed;


    private void Awake()
    {
        targetImage = GetComponent<Image>();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        elapsed = 0;
        targetImage.color = a;
    }

    protected override void OnUpdate()
    {
        elapsed += Time.deltaTime;

        float t = Mathf.PingPong(elapsed, lerpTime) / lerpTime;
        targetImage.color = Color.Lerp(a, b, t);
    }
}
