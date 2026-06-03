using Fire_Pixel.Utility;


/// <summary>
/// Static class containing game rules loaded from SO data.
/// </summary>
public static class GameRules
{
    public static CombatSettings CombatSettings { get; private set; }
    public static CompletionAction RulesInitComplete { get; set; } = new CompletionAction();


    public static void Reset()
    {
        RulesInitComplete = new CompletionAction();
        CombatSettings.Dispose();
    }
    public static void SetGameRules(CombatSettings combatSettings)
    {
        CombatSettings = combatSettings;

        RulesInitComplete?.Invoke();
    }
}