using UnityEngine;




[CreateAssetMenu(fileName = "New MoveSet", menuName = "ScriptableObjects/Combat/MoveSet", order = -1003)]
public class AttackMoveSetSO : ScriptableObject
{
    [SerializeField] private AttackSO[] Value;
    
    public AttackData[] GetMoveArray()
    {
        int moveCount = Value.Length;
        AttackData[] moveArray = new AttackData[moveCount];

        for (int i = 0; i < moveCount; i++)
        {
            moveArray[i] = Value[i].Value;
        }
        return moveArray;
    }
}