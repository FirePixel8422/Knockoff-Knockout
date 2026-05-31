using System;
using UnityEngine;


public class PlayerInputRouter : MonoBehaviour
{
    private PlayerInputHandler playerInputHandler;
    public bool IsAssigned;

//#if UNITY_EDITOR
    public Action<Vector2> DirectionInput;
    public Action<AttackInputFlags> AttackInput;
//#endif


    public void Init(PlayerInputHandler inputHandler)
    {
        playerInputHandler = inputHandler;
    }


    public void OnInputDeviceLost()
    {
        playerInputHandler.ClearInputBuffer(false);
        IsAssigned = false;
    }

    public void OnDirection(Vector2 dirVec)
    {
        playerInputHandler.OnDirection(dirVec);

//#if UNITY_EDITOR
        DirectionInput?.Invoke(dirVec);
//#endif
    }
    public void OnButtonPressed(AttackInputFlags flag)
    {
        playerInputHandler.OnButtonPressed(flag);

//#if UNITY_EDITOR
        AttackInput?.Invoke(flag);
//#endif
    }
}