using UnityEngine;


public class PlayerInputRouter : MonoBehaviour
{
    private PlayerInputHandler playerInputHandler;


    public void Init(PlayerInputHandler inputHandler)
    {
        playerInputHandler = inputHandler;
    }


    public void OnInputDeviceLost()
    {
        playerInputHandler.ClearInputBuffer(false);
    }

    public void OnDirection(Vector2 dirVec)
    {
        playerInputHandler.OnDirection(dirVec);
    }
    public void OnButtonPressed(AttackInputFlags flag)
    {
        playerInputHandler.OnButtonPressed(flag);
    }
}