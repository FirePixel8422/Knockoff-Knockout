using TMPro;
using UnityEngine;


public class AutoSizeBackground : UpdateMonoBehaviour
{
    [SerializeField] private RectTransform background;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float extraSizeDeltaX;
    private float initialRightEdgeX = 20;


    private void Awake()
    {
        // Calculate the initial right edge position in local space
        initialRightEdgeX = background.anchoredPosition.x + background.sizeDelta.x * (1 - background.pivot.x);
    }
    private void Start()
    {
        text.ForceMeshUpdate();
        UpdateUIScaling();
    }

    protected override void OnUpdate() => UpdateUIScaling();
    private void UpdateUIScaling()
    {
        // Get current text width and scale background to it
        float textWidth = text.GetRenderedValues(false).x;
        Vector2 size = background.sizeDelta;
        size.x = textWidth + extraSizeDeltaX;
        background.sizeDelta = size;

        // Keep the **right edge fixed**, adjust anchoredPosition
        float pivotX = background.pivot.x;
        Vector2 anchoredPos = background.anchoredPosition;
        anchoredPos.x = initialRightEdgeX - size.x * (1 - pivotX);
        background.anchoredPosition = anchoredPos;
    }
}
