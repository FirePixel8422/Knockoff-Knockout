using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class MenuInputTrigger : MonoBehaviour
{
    [SerializeField] private InputActionReference triggerInput;
    [SerializeField] private Button.ButtonClickedEvent onTrigger;


    private void OnEnable()
    {
        triggerInput.action.Enable();
        triggerInput.action.performed += OnExitMenu;
    }
    private void OnDisable()
    {
        triggerInput.action.performed -= OnExitMenu;
        triggerInput.action.Disable();
    }

    private void OnExitMenu(InputAction.CallbackContext ctx)
    {
        if (ctx.performed == false) return;

        onTrigger?.Invoke();
    }
}