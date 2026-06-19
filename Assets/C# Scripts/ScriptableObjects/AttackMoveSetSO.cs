using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;



[CreateAssetMenu(fileName = "New MoveSet", menuName = "ScriptableObjects/Combat/MoveSet", order = -1003)]
public class AttackMoveSetSO : ScriptableObject
{
    [SerializeField] private AttackSO[] baseMoves;
    
    /// <summary>
    /// Get all base attacks and string attacks as <see cref="AttackData"/> copies and bake all data into them
    /// </summary>
    public void GetBakedAttackData(out AttackData[] moveSet, out AttackData[] stringSet)
    {
        int moveCount = baseMoves.Length;
        moveSet = new AttackData[moveCount];

        HashSet<AttackSO> stringMoves = new HashSet<AttackSO>(moveCount);
        List<AttackSO> toCheckList = new List<AttackSO>();

        // Collect (duplicate free) set of all string attacks
        for (int i = 0; i < moveCount; i++)
        {
            AddStringTransitions(stringMoves, toCheckList, baseMoves[i].Value.StringTransitions);
        }

        // Collect all nested string attacks (in string attacks)
        while (toCheckList.Count > 0)
        {
            int toRemove = toCheckList.Count - 1;

            AddStringTransitions(stringMoves, toCheckList, toCheckList[toRemove].Value.StringTransitions);

            toCheckList.RemoveAtSwapBack(toRemove);
        }

        int stringMoveCount = stringMoves.Count;
        stringSet = new AttackData[stringMoveCount];

        // Send all string attacks in hashset to 'stringSet' (output)
        int i3 = 0;
        foreach (AttackSO attack in stringMoves)
        {
            attack.Value.AttackId = i3;

            stringSet[i3] = attack.Value;
            stringSet[i3].BakeAllCurves();
            i3 += 1;
        }

        for (int i = 0; i < moveCount; i++)
        {
            moveSet[i] = baseMoves[i].Value;
            moveSet[i].BakeAllCurves();
        }
    }

    /// <summary>
    /// Add all attacks in <paramref name="stringTransitions"/> to both <paramref name="stringMoves"/> and <paramref name="toCheckList"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddStringTransitions(HashSet<AttackSO> stringMoves, List<AttackSO> toCheckList, EditorStringTransition[] stringTransitions)
    {
        int stringCount = stringTransitions.Length;
        for (int i = 0; i < stringCount; i++)
        {
            AttackSO targetMove = stringTransitions[i].TargetMove;

            stringMoves.Add(targetMove);
            toCheckList.Add(targetMove);
        }
    }
}