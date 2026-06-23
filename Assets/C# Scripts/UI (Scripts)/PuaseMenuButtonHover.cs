using TMPro;
using UnityEngine;

public class PuaseMenuButtonHover : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Color exitHoverColor;
    public void HoverEnterColor()
    {
        buttonText.color = Color.white;
    }
    public void HoverExitColor()
    {
        buttonText.color = exitHoverColor;
    }
}
