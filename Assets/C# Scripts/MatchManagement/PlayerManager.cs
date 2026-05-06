using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Manager MB class that assigns input devices to the players.
/// </summary>
public class PlayerManager : FrameTickUpdateMB
{
    public static PlayerManager Instance { get; private set; }
    private void Awake() => Instance = this;


    [SerializeField] private PlayerController[] players;
    [SerializeField] private Color[] playerColors;

    [SerializeField] private GamepadRumbleParameters onJoinRumble;

    private readonly Dictionary<PlayerInputBinder, PlayerInputRouter> binderToRouterMap = new(GlobalGameData.MAX_PLAYERS);


#if Enable_Debug_Systems
    [SerializeField] private bool logInputDeviceChanges = true;
    public bool LogInputDeviceChanges => logInputDeviceChanges;
#endif

    public Color[] PlayerColors => playerColors;


    //public Color GetPlayerColor(PlayerInputBinder binder)
    //{
    //    int playerId = 0;
    //    foreach (var kvp in binderToRouterMap)
    //    {
    //        if (kvp.Key == binder)
    //        {
    //            return playerColors[playerId];
    //        }
    //        playerId += 1;
    //    }

    //    DebugLogger.LogWarning("Player color reqeust failed. Requester '" + binder.name + "' isnt registered");
    //    return default;
    //}


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

        PlayerInputRouter router = players[targetPlayerId].InputRouter;

        //DebugLogger.Log($"Input driver '{}' bound to {router.name}", logInputDeviceChanges);

        binderToRouterMap[binder] = router;
        binder.Bind(router, targetPlayerId);

        if (router == null)
        {
            DebugLogger.LogError("No available player slot for '" + binder.name + "' There shouldnt be more binders then routers", logInputDeviceChanges);
            return;
        }

        if (device != null && device is Gamepad pad)
        {
            GamepadRumble.SetRumble(pad, onJoinRumble);
        }
    }

    #endregion


    /// <summary>
    /// Unbind all bound active input module (binders) from their players (drivers).
    /// </summary>
    public void UnbindAllPlayerInput()
    {
        foreach(var kvp in binderToRouterMap)
        {
            kvp.Key.Unbind();
        }
        binderToRouterMap.Clear();
    }


    // TickUpdate players tick dependent logic in order.
    protected override void OnTickUpdate()
    {
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            players[i].TickUpdateAttackIntersections();
        }
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            players[i].TickUpdateAttack();
        }
    }
}