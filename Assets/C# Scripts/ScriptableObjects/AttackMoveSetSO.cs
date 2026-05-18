using UnityEngine;



[CreateAssetMenu(fileName = "New MoveSet", menuName = "ScriptableObjects/Combat/MoveSet", order = -1003)]
public class AttackMoveSetSO : ScriptableObject
{
    [SerializeField] private AttackSO[] Attacks;
    
    /// <summary>
    /// Get all attack as <see cref="AttackData"/> copies and bake all data into it
    /// </summary>
    public AttackData[] GetAsDataArray()
    {
        int moveCount = Attacks.Length;
        AttackData[] attacksArray = new AttackData[moveCount];

        for (int i = 0; i < moveCount; i++)
        {
            attacksArray[i] = Attacks[i].Value;
            attacksArray[i].BakeAllCurves();
        }
        return attacksArray;
    }
}