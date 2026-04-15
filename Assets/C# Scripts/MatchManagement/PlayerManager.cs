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

    private Dictionary<InputDevice, PlayerController> deviceToPlayer = new(2);

#if Enable_Debug_Systems
    [SerializeField] private bool logInputDeviceChanges = true;
#endif


    private void Awake()
    {
        InputSystem.onDeviceChange += OnDeviceChanged;

        joinAction.action.Enable();
        joinAction.action.performed += OnPlayerJoined;
    }
    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChanged;

        joinAction.action.performed -= OnPlayerJoined;
        joinAction.action.Disable();
    }


    #region Connect/Disconnect Devices

    /// <summary>
    /// When a player presses the join button, try to assign their device to an available player slot.
    /// </summary>
    private void OnPlayerJoined(InputAction.CallbackContext ctx) => TryConnectDevice(ctx.control.device);
    private void TryConnectDevice(InputDevice device)
    {
        if (deviceToPlayer.ContainsKey(device))
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
            break;
        }
        if (player == null)
        {
            DebugLogger.LogWarning("No available player slot for device " + device, logInputDeviceChanges);
            return;
        }

        deviceToPlayer[device] = player;
        DebugLogger.Log($"Connected {device.displayName} to {player.name}", logInputDeviceChanges);


        if (device is Gamepad pad)
        {
            pad.SetMotorSpeeds(12, 5); // (lowFreq, highFreq)
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
        if (!deviceToPlayer.TryGetValue(device, out PlayerController player))
            return;

        DebugLogger.Log($"Device disconnected: {device.displayName}", logInputDeviceChanges);

        deviceToPlayer.Remove(device);
        player.IsAssigned = false;
    }

    #endregion


    /*
    public void OnButton1(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayer.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.OnButton1(ctx.performed);
        }
    }
    public void OnButton2(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayer.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.OnButton2(ctx.performed);
        }
    }
    public void OnButton3(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayer.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.OnButton3(ctx.performed);
        }
    }
    public void OnButton4(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayer.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.OnButton4(ctx.performed);
        }
    }
    public void OnButton5(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayer.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.OnButton5(ctx.performed);
        }
    }
    public void OnButton6(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayer.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.OnButton6(ctx.performed);
        }
    }
    public void OnDirection(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (deviceToPlayer.TryGetValue(ctx.control.device, out PlayerController player))
        {
            player.OnButton1(ctx.performed);
        }
    }
    */
}