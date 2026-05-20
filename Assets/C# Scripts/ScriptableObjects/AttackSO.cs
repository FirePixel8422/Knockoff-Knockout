using UnityEngine;



[CreateAssetMenu(fileName = "New Move", menuName = "ScriptableObjects/Combat/Attack", order = -1003)]
public class AttackSO : ScriptableObject
{
    public AttackData Value;


#if UNITY_EDITOR
    private void OnValidate()
    {
        Value.UpdateDebugData();
    }
#endif
}