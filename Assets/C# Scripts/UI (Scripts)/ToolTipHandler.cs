using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class TooltipHandler : UpdateMonoBehaviour
{
    [SerializeField] private GameObject tooltipPrefab;
    [SerializeField] private ToolTipsSO toolTipDataSO;

    private ToolTipWord[] toolTipWords;

    private Dictionary<string, int> wordToTooltipId;
    private Dictionary<string, string> wordColorHexLookup;

    private TextMeshProUGUI[] registeredTextGuis;

    private GameObject activeTooltip;
    private RectTransform tooltipRect;
    private TextMeshProUGUI tooltipText;

    private int lastTooltipId = -1;
    private TextMeshProUGUI lastText;


    private void Awake()
    {
        toolTipWords = toolTipDataSO.Data;

        if (toolTipWords.IsNullOrEmpty())
        {
            DebugLogger.Log("No Tooltips found in ToolTipSO");
            Destroy(this);
        } 

        // Build lookup dictionaries
        int toolTipWordCount = toolTipWords.Length;
        wordToTooltipId = new Dictionary<string, int>(toolTipWordCount);
        wordColorHexLookup = new Dictionary<string, string>(toolTipWordCount);
        for (int i = 0; i < toolTipWordCount; i++)
        {
            string lower = toolTipWords[i].word.ToLowerInvariant();
            wordToTooltipId[lower] = i;
            wordColorHexLookup[lower] = ColorUtility.ToHtmlStringRGB(toolTipWords[i].wordColor);
        }

        // Get all TMP children
        registeredTextGuis = GetComponentsInChildren<TextMeshProUGUI>(true);

        // Instantiate tooltip
        activeTooltip = Instantiate(tooltipPrefab, transform.root);
        activeTooltip.SetActive(false);

        tooltipRect = activeTooltip.GetComponent<RectTransform>();
        tooltipText = activeTooltip.GetComponentInChildren<TextMeshProUGUI>();
        tooltipRect.pivot = new Vector2(0.5f, 0.5f);
    }

    /// <summary>
    /// Fast recolor of all registered TMPs using precomputed hex colors
    /// </summary>
    public void UpdateColoredWords()
    {
        int tmpCount = registeredTextGuis.Length;

        for (int i = 0; i < tmpCount; i++)
        {
            TextMeshProUGUI tmpText = registeredTextGuis[i];
            tmpText.ForceMeshUpdate(); // only needed if text changed

            TMP_TextInfo textInfo = tmpText.textInfo;
            int wordCount = textInfo.wordCount;
            if (wordCount == 0) continue;

            string original = tmpText.text;
            StringBuilder sb = new StringBuilder(original.Length + 50); // extra space for color tags
            int lastIndex = 0;

            for (int w = 0; w < wordCount; w++)
            {
                TMP_WordInfo wordInfo = textInfo.wordInfo[w];
                int start = wordInfo.firstCharacterIndex;
                int length = wordInfo.characterCount;
                string word = original.Substring(start, length);

                // Append text between last word and this word
                sb.Append(original, lastIndex, start - lastIndex);

                // Append colored word if exists
                if (wordColorHexLookup.TryGetValue(word.ToLowerInvariant(), out string hex))
                {
                    sb.Append("<color=#").Append(hex).Append(">").Append(word).Append("</color>");
                }
                else
                {
                    sb.Append(word);
                }

                lastIndex = start + length;
            }

            // Append any remaining text after the last word
            if (lastIndex < original.Length)
                sb.Append(original, lastIndex, original.Length - lastIndex);

            tmpText.text = sb.ToString();
        }
    }

    protected override void OnUpdate()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        int tmpCount = registeredTextGuis.Length;
        for (int i = 0; i < tmpCount; i++)
        {
            TextMeshProUGUI tmpText = registeredTextGuis[i];
            if (tmpText == null) continue;

            int wordIndex = TMP_TextUtilities.FindIntersectingWord(tmpText, mousePos, null);
            if (wordIndex == -1) continue;

            TMP_WordInfo wordInfo = tmpText.textInfo.wordInfo[wordIndex];
            string hoveredWord = wordInfo.GetWord().ToLowerInvariant();

            if (!wordToTooltipId.TryGetValue(hoveredWord, out int tooltipId))
                continue;

            if (tooltipId == lastTooltipId && tmpText == lastText)
            {
                activeTooltip.SetActiveSmart(true);
                return;
            }

            lastTooltipId = tooltipId;
            lastText = tmpText;

            ToolTipWord tooltipData = toolTipWords[tooltipId];

            GetWordWorldBounds(tmpText, wordInfo, out Vector3 bl, out Vector3 tr);

            Vector3 wordMid = (bl + tr) * 0.5f;
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, wordMid);

            float hoveredWordHeight = Mathf.Abs(tr.y - bl.y);

            tooltipText.text = tooltipData.toolTip;
            tooltipText.textWrappingMode = TextWrappingModes.Normal;

            tooltipRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tooltipData.width);
            tooltipRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tooltipData.height);
                
            tooltipText.ForceMeshUpdate();

            Vector3 offset = new Vector3(
                0f,
                hoveredWordHeight * 0.5f + tooltipData.height * 0.5f,
                0f
            );

            Vector3 finalPos = screenPos + offset;

            finalPos.x = Mathf.Clamp(finalPos.x, tooltipData.width * 0.5f, screenSize.x - tooltipData.width * 0.5f);
            finalPos.y = Mathf.Clamp(finalPos.y, tooltipData.height * 0.5f, screenSize.y - tooltipData.height * 0.5f);

            activeTooltip.transform.position = finalPos;
            activeTooltip.SetActiveSmart(true);
            return;
        }

        activeTooltip.SetActiveSmart(false);
        lastTooltipId = -1;
        lastText = null;
    }
    private void GetWordWorldBounds(TextMeshProUGUI tmpText, TMP_WordInfo wordInfo, out Vector3 bl, out Vector3 tr)
    {
        TMP_TextInfo textInfo = tmpText.textInfo;
        bl = Vector3.positiveInfinity;
        tr = Vector3.negativeInfinity;

        int charCount = wordInfo.characterCount;
        int firstCharIndex = wordInfo.firstCharacterIndex;

        for (int i = 0; i < charCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[firstCharIndex + i];
            if (!charInfo.isVisible) continue;

            Vector3 charBL = tmpText.transform.TransformPoint(charInfo.bottomLeft);
            Vector3 charTR = tmpText.transform.TransformPoint(charInfo.topRight);

            bl = Vector3.Min(bl, charBL);
            tr = Vector3.Max(tr, charTR);
        }
    }
}
