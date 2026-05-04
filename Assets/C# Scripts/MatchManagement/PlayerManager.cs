using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Manager MB class that assigns input devices to the players.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    private void Awake() => Instance = this;


    [SerializeField] private PlayerInputRouter[] players;
    [SerializeField] private GamepadRumbleParameters onJoinRumble;

    private readonly Dictionary<PlayerInputBinder, PlayerInputRouter> binderToRouterMap = new(2);

#if Enable_Debug_Systems
    [SerializeField] private bool logInputDeviceChanges = true;
#endif


    /// <summary>
    /// Bind an input module (binder) to a player (router)
    /// </summary>
    public void TryBindPlayerInput(PlayerInputBinder binder, InputDevice device = null)
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
            binder.Bind(router);
            break;
        }
        if (router == null)
        {
            DebugLogger.LogError("No available player slot for '" + binder.name + "' There shouldnt be more binders then routers", logInputDeviceChanges);
            return;
        }

        binderToRouterMap[binder] = router;
        DebugLogger.Log($"Bound '{binder.name}' to {router.name}", logInputDeviceChanges);

        if (device != null && device is Gamepad pad)
        {
            StartCoroutine(GamepadRumble.Rumble(pad, onJoinRumble));
        }
    }
}