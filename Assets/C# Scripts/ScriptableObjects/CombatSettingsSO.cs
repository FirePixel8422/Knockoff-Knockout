using UnityEngine;



[CreateAssetMenu(fileName = "Combat Settings", menuName = "ScriptableObjects/Combat/CombatSettings", order = -1003)]
public class CombatSettingsSO : ScriptableObject
{
    public CombatSettings Value;

    private void OnValidate()
    {
        Value.UpdateDebugData();
    }
}