using UnityEngine;


public class PlayerInputRouter : MonoBehaviour
{
    [EditorReadOnly, SerializeField] private bool isAssigned;
    public bool IsAssigned
    {
        get => isAssigned;
        set => isAssigned = value;
    }


    private PlayerInputHandler playerInputHandler;


    public void Init(PlayerInputHandler inputHandler)
    {
        playerInputHandler = inputHandler;
    }



    // Input Callbacks
    public void OnDirection(Vector2 dirVec)
    {
        playerInputHandler.OnDirection(dirVec);
    }
    public void OnButtonPressed(AttackInputFlags flag)
    {
        playerInputHandler.OnButtonPressed(flag);
    }
}