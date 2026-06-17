using UnityEngine;



[CreateAssetMenu(fileName = "Combat Settings", menuName = "ScriptableObjects/Combat/CombatSettings", order = -1003)]
public class CombatSettingsSO : ScriptableObject
{
    [SerializeField] private CombatSettings value;

    public CombatSettings GetValue()
    {
        value.BakeAllCurves();

        return value;
    }
}