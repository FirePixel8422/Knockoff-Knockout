using UnityEngine;



/// <summary>
/// MB class responsible for loading SO data into GameRules, for quick static acces to those rules from anywhere.
/// </summary>
public class DataInitializer : MonoBehaviour
{
    [SerializeField] private CombatSettingsSO combatSettingsSO;


    private void Awake()
    {
        GameRules.SetGameRules(combatSettingsSO.Value);
    }
}