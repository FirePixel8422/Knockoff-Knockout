using UnityEngine;



[CreateAssetMenu(fileName = "New MoveSet", menuName = "ScriptableObjects/Combat/MoveSet", order = -1003)]
public class AttackMoveSetSO : ScriptableObject
{
    [SerializeField] private AttackSO[] Moves;
    
    public AttackData[] GetMoveArray()
    {
        int moveCount = Moves.Length;
        AttackData[] moveArray = new AttackData[moveCount];

        for (int i = 0; i < moveCount; i++)
        {
            moveArray[i] = Moves[i].Value;
        }
        return moveArray;
    }
}