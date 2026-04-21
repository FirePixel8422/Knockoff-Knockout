using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Manager MB class that assigns input devices to the players.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;
    [SerializeField] private InputActionReference joinAction;
    [SerializeField] private GamepadRumbleParameters onJoinRumble;

    private Dictionary<InputDevice, PlayerController> deviceToPlayerMap = new(2);

#if Enable_Debug_Systems
    [SerializeField] private bool logInputDeviceChanges = true;
#endif


    private void Awake()
    {
        InputSystem.onDeviceChange += OnDeviceChanged;

        joinAction.action.Enable();
        joinAction.action.performed += OnJoin;
    }
    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChanged;

        joinAction.action.performed -= OnJoin;
        joinAction.action.Disable();
    }


    #region Player Input Callbacks

    /// <summary>
    /// When a player presses the join button, try to assign their device to an available player slot.
    /// </summary>
    private void OnJoin(InputAction.CallbackContext ctx)
    {
        TryConnectDevice(ctx.control.device);
    }

    public void OnDirection(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayerMap.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnDirection(ctx.ReadValue<Vector2>());
        }
    }

    public void OnButton1(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayerMap.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButton(AttackInputFlags.B1, ctx.performed);
        }
    }
    public void OnButton2(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayerMap.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButton(AttackInputFlags.B2, ctx.performed);
        }
    }
    public void OnButton3(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayerMap.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButton(AttackInputFlags.B3, ctx.performed);
        }
    }
    public void OnButton4(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayerMap.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButton(AttackInputFlags.B4, ctx.performed);
        }
    }
    public void OnButton5(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayerMap.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButton(AttackInputFlags.B5, ctx.performed);
        }
    }
    public void OnButton6(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayerMap.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButton(AttackInputFlags.B6, ctx.performed);
        }
    }
    public void OnButton7(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayerMap.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButton(AttackInputFlags.B7, ctx.performed);
        }
    }
    public void OnButton8(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayerMap.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButton(AttackInputFlags.B8, ctx.performed);
        }
    }

    #endregion


    #region Connect/Disconnect Devices

    private void TryConnectDevice(InputDevice device)
    {
        if (deviceToPlayerMap.ContainsKey(device))
        {
            DebugLogger.Log("Device " + device + " is already assigned to a player, skipping...", logInputDeviceChanges);
            return;
        }

        PlayerController player = null;
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            if (players[i].IsAssigned) continue;

            player = players[i];
            player.IsAssigned = true;
            player.enabled = true;
            break;
        }
        if (player == null)
        {
            DebugLogger.LogWarning("No available player slot for device " + device, logInputDeviceChanges);
            return;
        }

        deviceToPlayerMap[device] = player;
        DebugLogger.Log($"Connected {device.displayName} to {player.name}", logInputDeviceChanges);

        if (device is Gamepad pad)
        {
            StartCoroutine(GamepadRumble.Rumble(pad, onJoinRumble));
        }
    }

    /// <summary>
    /// When a device is disconnected, removed, or disabled, unassign it from its player slot.
    /// </summary>
    private void OnDeviceChanged(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Disconnected:
            case InputDeviceChange.Removed:
            case InputDeviceChange.Disabled:
                DisconnectDevice(device);
                break;
        }
    }
    private void DisconnectDevice(InputDevice device)
    {
        if (!deviceToPlayerMap.TryGetValue(device, out PlayerController player))
            return;

        DebugLogger.Log($"Device disconnected: {device.displayName}", logInputDeviceChanges);

        deviceToPlayerMap.Remove(device);
        player.IsAssigned = false;
        player.enabled = false;
    }

    #endregion
}