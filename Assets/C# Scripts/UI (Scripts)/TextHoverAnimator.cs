using TMPro;
using UnityEngine;

public class TextHoverAnimator : UpdateMonoBehaviour
{
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Color enterHoverColor;
    [SerializeField] private Color exitHoverColor;

    [SerializeField] private float colorLerpSpeed;

    private ColorLerpState colorState;

    private void Awake()
    {
        colorState = new ColorLerpState(enterHoverColor);
    }

    public void HoverEnterColor()
    {
        colorState.Target = enterHoverColor;
    }
    public void HoverExitColor()
    {
        colorState.Target = exitHoverColor;
    }

    protected override void OnUpdate()
    {
        if (colorState.IsCompleted(0.001f)) return;

        colorState.Lerp(colorLerpSpeed * Time.deltaTime);
        buttonText.color = colorState.Current;
    }
}
