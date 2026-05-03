using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;


/// <summary>
/// Manager MB class that assigns input devices to the players.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;
    [SerializeField] private GamepadRumbleParameters onJoinRumble;

    private Dictionary<InputDevice, PlayerController> deviceToPlayerMap = new(2);
    private InputDevice desktopDevice;

#if Enable_Debug_Systems
    [SerializeField] private bool logInputDeviceChanges = true;
#endif


    private IDisposable joinListener;

    private void Awake()
    {
        desktopDevice = Keyboard.current;

        InputSystem.onDeviceChange += OnDeviceChanged;

        joinListener = InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChanged;

        joinListener?.Dispose();
    }


    #region Player Input Callbacks

    /// <summary>
    /// When a player presses the join button, try to assign their device to an available player slot.
    /// </summary>
    private void OnAnyButtonPress(InputControl control)
    {
        print("pre");

        InputDevice device = control.device;

        if (control is not ButtonControl)
            return;

        // Treat keyboard + mouse as one shared desktop player
        if (device is Mouse)
        {
            return;
        }

        if (device is Keyboard)
        {
            device = desktopDevice;
        }
        else if (device is not Gamepad)
        {
            return;
        }

        TryConnectDevice(device);
    }

    public void OnDirection(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;

        if (GetTargetPlayer(ctx.control.device, out PlayerController player))
        {
            Vector2 dirVec = ctx.ReadValue<Vector2>();

            player.InputHandler.OnDirection(dirVec);
        }
    }

    public void OnButton1(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (GetTargetPlayer(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButtonPressed(AttackInputFlags.B1);
        }
    }
    public void OnButton2(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (GetTargetPlayer(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButtonPressed(AttackInputFlags.B2);
        }
    }
    public void OnButton3(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (GetTargetPlayer(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButtonPressed(AttackInputFlags.B3);
        }
    }
    public void OnButton4(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (GetTargetPlayer(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButtonPressed(AttackInputFlags.B4);
        }
    }
    public void OnButton5(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (GetTargetPlayer(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButtonPressed(AttackInputFlags.B5);
        }
    }
    public void OnButton6(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (GetTargetPlayer(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButtonPressed(AttackInputFlags.B6);
        }
    }
    public void OnButton7(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (GetTargetPlayer(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButtonPressed(AttackInputFlags.B7);
        }
    }
    public void OnButton8(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (GetTargetPlayer(ctx.control.device, out PlayerController player))
        {
            player.InputHandler.OnButtonPressed(AttackInputFlags.B8);
        }
    }

    private bool GetTargetPlayer(InputDevice device, out PlayerController player)
    {
        if (device is Keyboard || device is Mouse)
        {
            device = desktopDevice;
        }

        return deviceToPlayerMap.TryGetValue(device, out player);
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
        if (device is Mouse)
        {
            return;
        }

        if (device is Keyboard)
        {
            device = desktopDevice;
        }

        if (!GetTargetPlayer(device, out PlayerController player))
        {
            DebugLogger.LogError("errror");

            return;
        }

        DebugLogger.Log($"Device disconnected: {device.displayName}", logInputDeviceChanges);

        deviceToPlayerMap.Remove(device);
        player.IsAssigned = false;
        player.enabled = false;
    }

    #endregion
}