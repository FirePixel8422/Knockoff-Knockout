#if UNITY_EDITOR
using TMPro;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FontSwapper : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset fontA;
    [SerializeField] private TMP_FontAsset fontB;

    [SerializeField] private bool swap;
    [SerializeField] private bool reverseSwap;

    
    [ContextMenu("Swap Fonts")]
    private void SwapFonts()
    {
        DebugLogger.Log("Swapping fonts...");

        TMP_Text[] texts = this.FindObjectsOfType<TMP_Text>(true);

        List<TMP_Text> targets = new List<TMP_Text>();

        for (int i = 0; i < texts.Length; i++)
        {
            TMP_Text text = texts[i];

            if (text.font == fontA)
            {
                targets.Add(text);
            }
        }

        if (targets.Count == 0)
        {
            DebugLogger.Log("No TMP_Text found using fontA.");
            return;
        }

        Undo.SetCurrentGroupName("Swap TMP Fonts");
        int group = Undo.GetCurrentGroup();

        Undo.RegisterCompleteObjectUndo(targets.ToArray(), "Swap TMP Fonts");

        for (int i = 0; i < targets.Count; i++)
        {
            TMP_Text text = targets[i];
            text.font = fontB;
            EditorUtility.SetDirty(text);
        }

        Undo.CollapseUndoOperations(group);

        DebugLogger.Log($"Swapped {targets.Count} fonts.");
    }


    [ContextMenu("Undo Swap Fonts")]
    private void UndoSwapFonts()
    {
        DebugLogger.Log("Undo swapping fonts...");

        TMP_Text[] texts = this.FindObjectsOfType<TMP_Text>(true);

        List<TMP_Text> targets = new List<TMP_Text>();

        for (int i = 0; i < texts.Length; i++)
        {
            TMP_Text text = texts[i];

            if (text.font == fontB)
            {
                targets.Add(text);
            }
        }

        if (targets.Count == 0)
        {
            DebugLogger.Log("No TMP_Text found using fontB.");
            return;
        }

        Undo.SetCurrentGroupName("Undo TMP Fonts Swap");
        int group = Undo.GetCurrentGroup();

        Undo.RegisterCompleteObjectUndo(targets.ToArray(), "Undo TMP Fonts Swap");

        for (int i = 0; i < targets.Count; i++)
        {
            TMP_Text text = targets[i];
            text.font = fontA;
            EditorUtility.SetDirty(text);
        }

        Undo.CollapseUndoOperations(group);

        DebugLogger.Log($"Restored {targets.Count} fonts.");
    }


    private void OnValidate()
    {
        if (swap)
        {
            swap = false;
            SwapFonts();
        }

        if (reverseSwap)
        {
            reverseSwap = false;
            UndoSwapFonts();
        }
    }
}
#endif
