using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;


/// <summary>
/// Manager MB class that assigns input devices to the players.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    private void Awake() => Instance = this;


    [SerializeField] private PlayerInputRouter[] players;
    [SerializeField] private Color[] playerColors;
    public PlayerInputRouter[] Players => players;

    [SerializeField] private GamepadRumbleParameters onJoinRumble;

    private readonly Dictionary<PlayerInputBinder, PlayerInputRouter> binderToRouterMap = new(2);


#if Enable_Debug_Systems
    [SerializeField] private bool logInputDeviceChanges = true;
#endif

    /// <summary>
    /// Get player color with <paramref name="binder"/>
    /// </summary>
    public Color GetPlayerColor(PlayerInputBinder binder)
    {
        int playerId = 0;
        foreach (var kvp in binderToRouterMap)
        {
            if (kvp.Key == binder)
            {
                return playerColors[playerId];
            }
            playerId += 1;
        }

        DebugLogger.LogWarning("Player color reqeust failed. Requester '" + binder.name + "' isnt registered");
        return default;
    }

    // Called when a player connects their controller and join by pressing the join key.
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        ControllerAssigner.Instance.AssignInputDevice(playerInput);
        return;
        if (playerInput.TryGetComponent(out PlayerInputBinder binder))
        {
            TryBindPlayerInput(binder, playerInput.devices[0]);
        }
    }
    // Called when a player their controller disconnects
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        ControllerAssigner.Instance.RemoveInputDevice(playerInput);

        if (playerInput.TryGetComponent(out PlayerInputBinder binder) &&
            binderToRouterMap.TryGetValue(binder, out PlayerInputRouter router))
        {
            DebugLogger.Log($"Input driver '{binderToRouterMap.Keys.ToList().IndexOf(binder)}' unbound from {router.name}", logInputDeviceChanges);

            Destroy(binder.gameObject);
            router.IsAssigned = false;
        }
    }

    /// <summary>
    /// Bind an input module (binder) to a player (router)
    /// </summary>
    private void TryBindPlayerInput(PlayerInputBinder binder, InputDevice device = null)
    {
        if (binderToRouterMap.ContainsKey(binder))
        {
            DebugLogger.Log("'" + binder.name + "' is already assigned to a player, skipping...", logInputDeviceChanges);
            return;
        }

        PlayerInputRouter router = null;
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            if (players[i].IsAssigned) continue;

            router = players[i];
            router.IsAssigned = true;

            DebugLogger.Log($"Input driver '{i}' bound to {router.name}", logInputDeviceChanges);

            binderToRouterMap[binder] = router;
            binder.Bind(router, i);
            break;
        }
        if (router == null)
        {
            DebugLogger.LogError("No available player slot for '" + binder.name + "' There shouldnt be more binders then routers", logInputDeviceChanges);
            return;
        }

        if (device != null && device is Gamepad pad)
        {
            StartCoroutine(GamepadRumble.Rumble(pad, onJoinRumble));
        }
    }
}