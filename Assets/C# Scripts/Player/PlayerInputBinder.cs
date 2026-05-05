using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputBinder : MonoBehaviour
{
    [EditorReadOnly, SerializeField] private PlayerInput playerInput;
    [EditorReadOnly, SerializeField] private PlayerInputRouter playerInputRouter;
    [EditorReadOnly, SerializeField] private bool isAssigned;
    [EditorReadOnly, SerializeField] private int playerId;
    public bool IsAssigned => isAssigned;



    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    /// <summary>
    /// Bind this input binder to a player object (inputrouter)
    /// </summary>
    public void Bind(PlayerInputRouter router, int playerId)
    {
        playerInputRouter = router;
        isAssigned = true;
        this.playerId = playerId;
    }

    // When the input device is disconnected, completely kick it out of the game to free the connected player slot
    public void OnDeviceLost(PlayerInput playerInput)
    {
        StartCoroutine(HandleDisconnect(playerInput));
    }

    private IEnumerator HandleDisconnect(PlayerInput playerInput)
    {
        yield return null;
        playerInput.user.UnpairDevicesAndRemoveUser();
        PlayerManager.Instance.OnPlayerLeft(playerInput);
    }

    #region Input Callbacks

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