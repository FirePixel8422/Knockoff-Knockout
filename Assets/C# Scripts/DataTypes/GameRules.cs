using UnityEngine;


/// <summary>
/// Static class containing game rules loaded from SO data.
/// </summary>
public static class GameRules
{
    public static CombatSettings CombatSettings { get; private set; }


    public static void SetGameRules(CombatSettings combatSettings)
    {
        CombatSettings = combatSettings;
    }
}