using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputBinder : MonoBehaviour
{
    [EditorReadOnly, SerializeField] private PlayerInputRouter playerInputRouter;
    [EditorReadOnly, SerializeField] private bool isAssigned;
    public bool IsAssigned => isAssigned;



    private void Awake()
    {
        PlayerManager.Instance.TryBindPlayerInput(this);
    }

    public void Bind(PlayerInputRouter router)
    {
        playerInputRouter = router;
        isAssigned = true;
        router.IsAssigned = true;

        print("'" + name + "' Bound to " + router.name);
    }
    public void Unbind()
    {
        isAssigned = false;
        playerInputRouter.IsAssigned = false;

        playerInputRouter = null;
    }

    private void OnDestroy()
    {
        Unbind();
    }


    #region Input Callbacks

    public void OnDirection(InputAction.CallbackContext ctx)
    {
        playerInputRouter.OnDirection(ctx.ReadValue<Vector2>());
    }

    public void OnButton1(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B1);
    }
    public void OnButton2(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B2);
    }
    public void OnButton3(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B3);
    }
    public void OnButton4(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B4);
    }
    public void OnButton5(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B5);
    }
    public void OnButton6(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B6);
    }
    public void OnButton7(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B7);
    }
    public void OnButton8(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        playerInputRouter.OnButtonPressed(AttackInputFlags.B8);
    }

    #endregion
}