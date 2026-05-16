using Fire_Pixel.Utility;


/// <summary>
/// Static class containing game rules loaded from SO data.
/// </summary>
public static class GameRules
{
    public static CombatSettings CombatSettings { get; private set; }
    public static OneTimeAction PostRulesInitialized { get; set; } = new OneTimeAction();


    public static void Reset()
    {
        PostRulesInitialized = new OneTimeAction();
    }
    public static void SetGameRules(CombatSettings combatSettings)
    {
        CombatSettings = combatSettings;

        PostRulesInitialized?.Invoke();
    }
}