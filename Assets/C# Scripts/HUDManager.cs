using UnityEngine;


/// <summary>
/// MB class responsible for updating the HUD/UI of the game
/// </summary>
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }


    [SerializeField] private PlayerUIModules[] modules;
    public PlayerUIModules GetPlayerUIModule(bool isRightPlayer)
    {
        return isRightPlayer ? modules[1] : modules[0];
    }


    private void Awake()
    {
        Instance = this;

        GameRules.PostRulesInitialized += () =>
        {
            for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
            {
                modules[i].HealthBar.Init();
            }
        };
    }

    public void UpdateUI(float deltaTime)
    {
        float globalTime = Time.time;

        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            modules[i].HealthBar.OnUpdate(deltaTime, globalTime);
        }
    }
}


[System.Serializable]
public struct PlayerUIModules
{
    public HealthBarController HealthBar;
}