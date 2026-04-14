using UnityEngine;


[System.Serializable]
public struct ToolTipWord
{
    public string word;
    public Color wordColor;

    [TextArea]
    public string toolTip;

    [Header("Layout")]
    public float width;
    public float height;
}