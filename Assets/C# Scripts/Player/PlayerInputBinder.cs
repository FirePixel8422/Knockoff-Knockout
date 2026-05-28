using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputBinder : MonoBehaviour
{
    [EditorReadOnly, SerializeField] private PlayerInputRouter playerInputRouter;
    [EditorReadOnly, SerializeField] private bool isAssigned;
    [EditorReadOnly, SerializeField] private int playerId;
    public bool IsAssigned => isAssigned;
    public int PlayerId => playerId;


    #region Bind/Unbind To Player (PlayerInputRouter)

    /// <summary>
    /// Bind this input binder to a player object (input router)
    /// </summary>
    public void Bind(PlayerInputRouter router, int playerId)
    {
        playerInputRouter = router;
        playerInputRouter.IsAssigned = true;

        isAssigned = true;
        this.playerId = playerId;
    }
    /// <summary>
    /// Unbind this input binder from assigned to player (input router) and reset all values to default.
    /// </summary>
    public void Unbind()
    {
        if (playerInputRouter != null)
        {
            playerInputRouter.OnInputDeviceLost();
        }
        playerInputRouter = null;

        isAssigned = false;
        playerId = -1;
    }

    #endregion


    #region OnDeviceLost Handling

    // When the input device is disconnected, completely kick it out of the game to free the connected player slot
    public void OnDeviceLost(PlayerInput playerInput)
    {
        // Send removal to custom systems first before destroying the input device.
        ControllerAssigner.Instance.RemoveInputDevice(playerInput);
        DebugLogger.Log($"Device {playerInput.devices[0].displayName} lost, destroying input driver", PlayerManager.Instance.LogInputDeviceChanges);

        // Send destroy call to playermanager after 1 frame to give custom systems a chance to execute code while the input device is still alive.
        StartCoroutine(HandleDisconnect(playerInput));
    }

    private IEnumerator HandleDisconnect(PlayerInput playerInput)
    {
        yield return null;
        playerInput.user.UnpairDevicesAndRemoveUser();
        PlayerManager.Instance.OnPlayerLeft(playerInput);
    }

    #endregion


    #region Input Callbacks

    public void OnStart(InputAction.CallbackContext ctx)
    {
        if (!IsAssigned || !ctx.performed) return;
    }
    public void OnDirection(InputAction.CallbackContext ctx)
    {
        if (!isAssigned) return;

        playerInputRouter.OnDirection(ctx.ReadValue<Vector2>());
    }

    public void OnButton1(InputAction.CallbackContext ctx)
    {
        if (!isAssigned || !ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B1);
    }
    public void OnButton2(InputAction.CallbackContext ctx)
    {
        if (!isAssigned || !ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B2);
    }
    public void OnButton3(InputAction.CallbackContext ctx)
    {
        if (!isAssigned || !ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B3);
    }
    public void OnButton4(InputAction.CallbackContext ctx)
    {
        if (!isAssigned || !ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B4);
    }
    public void OnButton5(InputAction.CallbackContext ctx)
    {
        if (!isAssigned || !ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B5);
    }
    public void OnButton6(InputAction.CallbackContext ctx)
    {
        if (!isAssigned || !ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B6);
    }
    public void OnButton7(InputAction.CallbackContext ctx)
    {
        if (!isAssigned || !ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B7);
    }
    public void OnButton8(InputAction.CallbackContext ctx)
    {
        if (!isAssigned || !ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B8);
    }

    #endregion
}