using UnityEngine;



[CreateAssetMenu(fileName = "New Move", menuName = "ScriptableObjects/Combat/Attack", order = -1003)]
public class AttackSO : ScriptableObject
{
    public AttackData Value;
    

    private void OnValidate()
    {
        Value.UpdateDebugData();
    }
}