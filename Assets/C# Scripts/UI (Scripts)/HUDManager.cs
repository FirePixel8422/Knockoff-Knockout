using Fire_Pixel.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// MB class responsible for updating the HUD/UI of the game
/// </summary>
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }


    [SerializeField] private PlayerUIModule[] modules;

    [SerializeField] private TextMeshProUGUI stockWonText;
    [SerializeField] private Animator stockWonAnim;

    [SerializeField] private TextMeshProUGUI matchWonText;
    [SerializeField] private Animator matchWonAnim;


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
        if (MatchManager.Instance.GameState == GameState.PreGame || MatchManager.Instance.IsMatchRestarting) return;

        if (modules[isLeftPlayer ? 1 : 0].Stocks.WillWin)
        {
            EndGame(isLeftPlayer);

            modules[isLeftPlayer ? 1 : 0].Stocks.AddStock();
            return;
        }

        stockWonText.text = isLeftPlayer ? "Blue Won a Stock" : "Red Won a Stock";
        stockWonAnim.CrossFadeInFixedTime("ScaleInPopup", 0);

        modules[isLeftPlayer ? 1 : 0].Stocks.AddStock();
    }

    public void EndGame(bool isLeftPlayer)
    {
        matchWonText.text = isLeftPlayer ? "Blue Won the Match" : "Red Won the Match";
        matchWonAnim.CrossFadeInFixedTime("ScaleInPopupLong", 0);

        this.Invoke(3, () =>
        {
            MatchManager.Instance.StartMatch();
        });
    }
}


[System.Serializable]
public class PlayerUIModule
{
    public HealthBarUIController HealthBar;
    public StockUIController Stocks;
}