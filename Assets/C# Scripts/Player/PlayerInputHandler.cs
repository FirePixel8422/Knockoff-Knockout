using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for handling player moves 
/// </summary>
[System.Serializable]
public class PlayerInputHandler
{
    [EditorReadOnly, SerializeField] private AttackData[] moveSet;
    [EditorReadOnly, SerializeField] private InputBufferHandler bufferHandler = new();


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


    #region Buffer Update/Managament

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
        cRawInput.AttackFlags = AttackInputFlags.None;

        index.IncrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
    }

    #endregion


    /// <summary>
    /// Check all moves to see if input buffer correlates to one
    /// </summary>
    public bool NAME_TBD(AttackData[] moveSet, out AttackData targetMove)
    {
        int bestMoveStrength = 0;
        targetMove = new AttackData();

        int moveSetLength = moveSet.Length;
        for (int i = 0; i < moveSetLength; i++)
        {
            int moveStrength = TestInput(moveSet[i].Input);

            if (moveStrength <= bestMoveStrength)
                continue;

            bestMoveStrength = moveStrength;
            targetMove = moveSet[i];
        }

        return bestMoveStrength != 0;
    }
    /// <summary>
    /// Check if input is found in buffer for inputted move their Keybinds (FrameInput)
    /// </summary>
    private int TestInput(FrameInput targetInput)
    {
        int moveStrength = 0;
        int bufferIndex = index;
        bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);

        for (int i = 0; i < GlobalGameData.INPUT_BUFFER_SIZE; i++)
        {
            // Check if buffered input contains the same attack buttons
            if (!inputBuffer[bufferIndex].AttackFlags.HasFlag(targetInput.AttackFlags))
            {
                bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
                continue;
            }

            // If button is correct and direction is neutral, award 2/3 points
            if (targetInput.DirectionFlag == DirectionInputFlag.Neutral)
            {
                moveStrength = 2;
                continue;
            }

            int dirIndex = bufferIndex;
            for (int j = 0; j < GlobalGameData.DIRECTION_BUFFER_WINDOW; j++)
            {
                // Check into the past of the buffer for X frames for if the target attacks direction is found
                if (inputBuffer[dirIndex].DirectionFlag == targetInput.DirectionFlag)
                {
                    // Exact move was matched with buffer history, award 3/3 points
                    return 3;
                }
                dirIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
            }

            bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
        }
        return moveStrength;
    }
}