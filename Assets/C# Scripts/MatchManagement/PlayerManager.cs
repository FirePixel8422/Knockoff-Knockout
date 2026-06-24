using Fire_Pixel.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Manager MB class that assigns input devices to the Players.
/// </summary>
public class PlayerManager : FrameTickUpdateMB
{
    public static PlayerManager Instance { get; private set; }
    public static CompletionAction PlayersInitComplete { get; set; } = new CompletionAction();


    [SerializeField] private AttackMoveSetSO moveSetSO;
    [SerializeField] private Transform[] playerSpawnPoints;
    [field: SerializeField] public PlayerController[] Players { get; private set; }
    [field: SerializeField] public Color[] PlayerColors { get; private set; }


    private readonly Dictionary<PlayerInputBinder, PlayerInputRouter> binderToRouterMap = new(GlobalGameData.MAX_PLAYERS);

    private readonly bool[] playerAttackConnects = new bool[GlobalGameData.MAX_PLAYERS];


#if Enable_Debug_Systems
    [SerializeField] private bool logInputDeviceChanges = true;
    public bool LogInputDeviceChanges => logInputDeviceChanges;
#endif


    private void Awake()
    {
        Instance = this;

        GameRules.RulesInitComplete += () =>
        {
            for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
            {
                moveSetSO.GetBakedAttackData(out AttackData[] moveSet, out AttackData[] stringSet);
                Players[i].Init(moveSet, stringSet);
            }

            PlayersInitComplete?.Invoke();
        };
    }
    private void OnDestroy()
    {
        PlayersInitComplete = new CompletionAction();
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            Players[i].Dispose();
        }
    }


    public void ResetPlayers()
    {
        CameraManager.Instance.ResetTransform();

        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            Players[i].StateMachine.ResolveAttack(new AttackData
            {
                Damage = 1000
            }, AttackResult.KnockDown, true);

            Players[i].MovementHandler.SetTransform(playerSpawnPoints[i].position, playerSpawnPoints[i].rotation);
        }
    }


    #region Player Join/Leave Callbacks

    // Called when a player connects their controller and join by pressing the join key.
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        ControllerAssigner.Instance.AssignInputDevice(playerInput);
        DebugLogger.Log($"Created input driver for {playerInput.devices[0].displayName}", logInputDeviceChanges);
    }
    // Called when a player their controller disconnects
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        if (playerInput.TryGetComponent(out PlayerInputBinder binder))
        {
            Destroy(binder.gameObject);

            if (binderToRouterMap.TryGetValue(binder, out PlayerInputRouter router))
            {
                DebugLogger.Log($"Input driver '{binderToRouterMap.Keys.ToList().IndexOf(binder)}' unbound from {router.name}", logInputDeviceChanges);
            }
        }
    }

    #endregion


    #region Bind/Unbind Binder (Input) to Router (Player)

    /// <summary>
    /// Bind an input module (binder) to a player (router)
    /// </summary>
    public void BindPlayerInput(PlayerInputBinder binder, int targetPlayerId, InputDevice device = null)
    {
        if (binderToRouterMap.ContainsKey(binder))
        {
            DebugLogger.Log("'" + binder.name + "' is already assigned to a player, skipping...", logInputDeviceChanges);
            return;
        }

        PlayerInputRouter router = Players[targetPlayerId].InputRouter;

        //DebugLogger.Log($"Input driver '{}' bound to {router.name}", logInputDeviceChanges);

        binderToRouterMap[binder] = router;
        binder.Bind(router, targetPlayerId);

        if (router == null)
        {
            DebugLogger.LogError("No available player slot for '" + binder.name + "' There shouldnt be more binders then routers", logInputDeviceChanges);
            return;
        }
    }

    /// <summary>
    /// Unbind all bound active input module (binders) from their Players (drivers).
    /// </summary>
    public void UnbindAllPlayerInput()
    {
        foreach(var kvp in binderToRouterMap)
        {
            kvp.Key.Unbind();
        }
        binderToRouterMap.Clear();
    }

    #endregion


    #region TickUpdate and Update

    protected override void OnUpdate()
    {
        if (MatchManager.Instance.IsGamePaused) return;

        float deltaTime = Time.deltaTime;

        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            Players[i].OnUpdate(deltaTime);
        }

        // Update systems after Players.
        HUDManager.Instance.UpdateUI(deltaTime);
        CameraManager.Instance.UpdateCamera(deltaTime);
    }


    // TickUpdate Players tick dependent logic in order.
    protected override void OnTickUpdate()
    {
        if (MatchManager.Instance.IsGamePaused) return;

        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            Players[i].PreTickUpdate();
        }
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            Players[i].TickUpdate(out playerAttackConnects[i]);
        }
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            Players[i].PostTickUpdate(playerAttackConnects[i]);
        }

        PlayerSpacingManager.Instance.TickUpdate();
        CameraManager.Instance.TickUpdate();
    }

    #endregion
}