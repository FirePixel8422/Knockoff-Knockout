using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ButtonInputTrigger : MonoBehaviour
{
    [SerializeField] private InputActionReference triggerInput;
    private Button button;


    private void Awake()
    {
        button = GetComponent<Button>();
    }
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

        button.onClick?.Invoke();
    }
}