using UnityEngine;



[CreateAssetMenu(fileName = "New MoveSet", menuName = "ScriptableObjects/Combat/MoveSet", order = -1003)]
public class AttackMoveSetSO : ScriptableObject
{
    [SerializeField] private AttackSO[] baseMoves;
    [SerializeField] private AttackSO[] stringMoves;
    
    /// <summary>
    /// Get all base attacks and string attacks as <see cref="AttackData"/> copies and bake all data into them
    /// </summary>
    public void GetBakedDataArrays(out AttackData[] moveSet, out AttackData[] stringSet)
    {
        int moveCount = baseMoves.Length;
        moveSet = new AttackData[moveCount];

        for (int i = 0; i < moveCount; i++)
        {
            moveSet[i] = baseMoves[i].Value;
            moveSet[i].BakeAllCurves();
        }

        int stringMoveCount = stringMoves.Length;
        stringSet = new AttackData[stringMoveCount];

        for (int i = 0; i < stringMoveCount; i++)
        {
            stringSet[i] = stringMoves[i].Value;
            stringSet[i].BakeAllCurves();
        }
    }
}