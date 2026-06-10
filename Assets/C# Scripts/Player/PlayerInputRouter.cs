using UnityEngine;


/// <summary>
/// MB class recieving input callbacks through a bound <see cref="PlayerInputBinder"/> component.
/// </summary>
public class PlayerInputRouter : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    private PlayerInputOverrider inputOverrider;
    public bool IsAssigned;


    public void Init(PlayerInputHandler inputHandler, PlayerInputOverrider inputOverrider)
    {
        this.inputHandler = inputHandler;
        this.inputOverrider = inputOverrider;
    }


    public void OnInputDeviceLost()
    {
        inputHandler.ClearInputBuffer(false);
        IsAssigned = false;
    }

    public void OnDirection(Vector2 dirVec)
    {
        inputHandler.OnDirection(dirVec);
#if UNITY_EDITOR
        inputOverrider.OnDirection(dirVec);
#endif
    }
    public void OnButtonPressed(AttackInputFlags flag)
    {
        inputHandler.OnButtonPressed(flag);
#if UNITY_EDITOR
        inputOverrider.OnButtonPressed(flag);
#endif
    }
}