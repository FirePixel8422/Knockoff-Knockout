using UnityEngine;


/// <summary>
/// MB class responsible for updating the HUD/UI of the game
/// </summary>
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }


    [SerializeField] private PlayerUIModule[] modules;


    private void Awake()
    {
        Instance = this;

        PlayerManager.PlayersInitComplete += () =>
        {
            FighterSettings fighterSettings = GameRules.CombatSettings.Fighter;

            PlayerController targetPlayer;
            PlayerUIModule targetUIModule;

            ResetHUD();

            for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
            {
                targetPlayer = PlayerManager.Instance.Players[i];
                targetUIModule = modules[i];

                targetUIModule.HealthBar.Init(fighterSettings.StartHealth);
                targetPlayer.HealthHandler.OnHealthChanged += targetUIModule.HealthBar.OnHealthChanged;
                targetPlayer.HealthHandler.OnFighterDied += AddStock;
            }
        };
    }

    public void ResetHUD()
    {
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            modules[i].Stocks.ResetStocks();
        }
    }

    public void UpdateUI(float deltaTime)
    {
        float globalTime = Time.time;

        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            modules[i].HealthBar.OnUpdate(deltaTime, globalTime);
        }
    }

    public void AddStock(bool isLeftPlayer)
    {
        modules[isLeftPlayer ? 1 : 0].Stocks.AddStock();
    }
}


[System.Serializable]
public class PlayerUIModule
{
    public HealthBarUIController HealthBar;
    public StockUIController Stocks;
}