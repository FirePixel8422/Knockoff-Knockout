using System.Collections.Generic;
using UnityEngine;


public class PlayerControllerOverride : FrameTickUpdateMB
{
    [SerializeField] private List<FrameInput> inputBuffer = new List<FrameInput>();
    private int index;

    [SerializeField] private bool collectInputs;

    private FrameInput cRawInput;

    private PlayerInputRouter playerInputRouter;


    private void Awake()
    {
        playerInputRouter = GetComponent<PlayerInputRouter>();

        if (collectInputs)
        {
            playerInputRouter.DirectionInput += OnDirection;
            playerInputRouter.AttackInput += OnButtonPressed;
        }
    }
    private void OnDestroy()
    {
        playerInputRouter.DirectionInput -= OnDirection;
        playerInputRouter.AttackInput -= OnButtonPressed;
    }

    public void OnDirection(Vector2 dirVec)
    {
        if (!isActiveAndEnabled) return;

        DirectionInput dirInput;

        if (dirVec == Vector2.zero)
        {
            dirInput = DirectionInput.Neutral;
        }
        else if (Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))
        {
            dirInput = dirVec.x >= 0
                ? DirectionInput.Right
                : DirectionInput.Left;
        }
        else
        {
            dirInput = dirVec.y >= 0
                ? DirectionInput.Up
                : DirectionInput.Down;
        }

        cRawInput.DirectionFlag = dirInput;
    }
    public void OnButtonPressed(AttackInputFlags flag)
    {
        if (!isActiveAndEnabled) return;

        cRawInput.AttackFlags |= flag;
    }

    protected override void OnTickUpdate()
    {
        if (!isActiveAndEnabled) return;

        if (collectInputs)
        {
            inputBuffer.Add(cRawInput);
            cRawInput.AttackFlags = AttackInputFlags.None;
            return;
        }

        if (index == inputBuffer.Count) return;

        FrameInput input = inputBuffer[index++];

        Vector3 dir = input.DirectionFlag switch
        {
            DirectionInput.Left => Vector2.left,
            DirectionInput.Right => Vector2.right,
            DirectionInput.Up => Vector2.up,
            DirectionInput.Down => Vector2.down,
            _ => Vector2.zero
        };

        playerInputRouter.OnDirection(dir);
        playerInputRouter.OnButtonPressed(input.AttackFlags);
    }
}
