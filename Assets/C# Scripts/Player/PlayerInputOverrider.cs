using System.Collections.Generic;
using UnityEngine;


public class PlayerInputOverrider : MonoBehaviour
{
    [SerializeField] private OverrideMode mode;
    [SerializeField] private List<FrameInput> inputBuffer = new List<FrameInput>();

    private int index;
    private FrameInput cRawInput;

    private PlayerInputRouter playerInputRouter;


    private void Awake()
    {
        playerInputRouter = GetComponent<PlayerInputRouter>();

        playerInputRouter.DirectionInput += OnDirection;
        playerInputRouter.AttackInput += OnButtonPressed;
    }
    private void OnDestroy()
    {
        playerInputRouter.DirectionInput -= OnDirection;
        playerInputRouter.AttackInput -= OnButtonPressed;
    }

    public void OnDirection(Vector2 dirVec)
    {
        if (mode != OverrideMode.Collect) return;

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
        if (mode != OverrideMode.Collect) return;

        cRawInput.AttackFlags |= flag;
    }

    public void CollectInputs()
    {
        switch (mode)
        {
            case OverrideMode.None:
            default:
                return;

            case OverrideMode.Collect:
                if (!playerInputRouter.IsAssigned) return;
                inputBuffer.Add(cRawInput);
                cRawInput.AttackFlags = AttackInputFlags.None;
                break;

            case OverrideMode.FirstConstant:
                if (playerInputRouter.IsAssigned || inputBuffer.Count == 0) return;
                SendInput(inputBuffer[0]);
                break;

            case OverrideMode.Playback:
                if (playerInputRouter.IsAssigned || index == inputBuffer.Count) return;
                SendInput(inputBuffer[index++]);
                break;
            case OverrideMode.PlaybackLoop:
                if (playerInputRouter.IsAssigned || index == inputBuffer.Count) return;
                SendInput(inputBuffer[index]);
                index.IncrementSmart(inputBuffer.Count);
                break;
        }
    }
    private void SendInput(FrameInput input)
    {
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


    private enum OverrideMode
    {
        None,
        Collect,
        FirstConstant,
        Playback,
        PlaybackLoop,
    }
}
