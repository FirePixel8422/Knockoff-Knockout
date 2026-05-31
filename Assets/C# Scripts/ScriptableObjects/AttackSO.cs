using UnityEngine;



[CreateAssetMenu(fileName = "New Move", menuName = "ScriptableObjects/Combat/Attack", order = -1003)]
public class AttackSO : ScriptableObject
{
#if UNITY_EDITOR
    public string animationName;
    public int blendIn;
    public int blendOut;
#endif

    public AttackData Value;


#if UNITY_EDITOR
    private void OnValidate()
    {
        Value.UpdateDebugData(animationName, blendIn, blendOut);
    }
#endif
}