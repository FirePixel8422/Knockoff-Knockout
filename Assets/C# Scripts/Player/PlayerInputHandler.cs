using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for handling player moves 
/// </summary>
[System.Serializable]
public class PlayerInputHandler
{
    [SerializeField] private InputBufferHandler bufferHandler = new InputBufferHandler();


    /// <summary>
    /// Called by the PlayerInput component when an input is performed or canceled, with that corresponding button
    /// </summary>
    public void OnButton(AttackInputFlags flag, bool performed)
    {
        bufferHandler.UpdateCurrentInput(flag, performed);
    }
    /// <summary>
    /// Called by the PlayerInput component when the directional input is performed or canceled, with the current direction.
    /// </summary>
    public void OnDirection(Vector2 dirVec)
    {
        DirectionInputFlag dirFlag;

        if (dirVec == Vector2.zero)
        {
            dirFlag = DirectionInputFlag.Neutral;
        }
        else if (Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))
        {
            dirFlag = dirVec.x >= 0
                ? DirectionInputFlag.Right
                : DirectionInputFlag.Left;
        }
        else 
        {
            dirFlag = dirVec.y >= 0
                ? DirectionInputFlag.Up
                : DirectionInputFlag.Down;
        }

        bufferHandler.UpdateCurrentDirection(dirFlag);
    }

    /// <summary>
    /// Push all collected input from the last tick to the current one into the input buffer
    /// </summary>
    public void CollectInputs() => bufferHandler.PushBuffer();
}

[System.Serializable]
public class InputBufferHandler
{
    [SerializeField] private FrameInput[] inputBuffer = new FrameInput[GlobalGameData.INPUT_BUFFER_SIZE];
    private int index;

    [SerializeField] private FrameInput cRawInput;


    public void UpdateCurrentInput(AttackInputFlags flag, bool state)
    {
        if (state == true)
        {
            cRawInput.AttackFlags |= flag;
        }
        else
        {
            cRawInput.AttackFlags &= ~flag;
        }
    }
    public void UpdateCurrentDirection(DirectionInputFlag dir)
    {
        cRawInput.DirectionFlag = dir;
    }

    public void PushBuffer()
    {
        // Write collected input to buffer
        inputBuffer[index] = cRawInput;

        index.IncrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
    }

    public bool TestInputForMove(FrameInput targetInput)
    {
        int targetIndex = index;
        for (int i = 0; i < inputBuffer.Length; i++)
        {
            FrameInput bufferedInput = inputBuffer[targetIndex];

            // Check if buffered input has the same direction and contains the same buttons (or more)
            if (bufferedInput.DirectionFlag == targetInput.DirectionFlag &&
                bufferedInput.AttackFlags.HasFlag(targetInput.AttackFlags))
            {
                return true;
            }

            targetIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
        }

        // Move input wasnt found in buffer
        return false;
    }
}