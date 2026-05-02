using UnityEngine;



[CreateAssetMenu(fileName = "New MoveSet", menuName = "ScriptableObjects/Combat/MoveSet", order = -1003)]
public class AttackMoveSetSO : ScriptableObject
{
    [SerializeField] private AttackSO[] Attacks;
    
    public AttackData[] GetAttacksArray()
    {
        int moveCount = Attacks.Length;
        AttackData[] moveArray = new AttackData[moveCount];

        for (int i = 0; i < moveCount; i++)
        {
            moveArray[i] = Attacks[i].Value;
        }
        return moveArray;
    }
}