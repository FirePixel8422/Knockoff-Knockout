using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for handling player moves 
/// </summary>
[System.Serializable]
public class PlayerInputHandler
{
    [EditorReadOnly, SerializeField] private AttackData[] moveSet;
    [EditorReadOnly, SerializeField] private InputBufferHandler bufferHandler;


    public PlayerInputHandler(AttackData[] moveSet)
    {
        this.moveSet = moveSet;
        bufferHandler = new InputBufferHandler();
    }
    private PlayerInputHandler() { }
    ~PlayerInputHandler()
    {
        int attackCount = moveSet.Length;
        for (int i = 0; i < attackCount; i++)
        {
            moveSet[i].Dispose();
        }
    }

    
    #region Player Input Callbacks

    /// <summary>
    /// Send button input <paramref name="flag"/> to the input buffer
    /// </summary>
    public void OnButtonPressed(AttackInputFlags flag)
    {
        bufferHandler.UpdateCurrentInput(flag);
    }
    /// <summary>
    /// Send direction input <paramref name="dirVec"/> to the input buffer
    /// </summary>
    public void OnDirection(Vector2 dirVec)
    {
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

        bufferHandler.UpdateCurrentDirection(dirInput);
    }

    #endregion


    #region Input Buffer Interaction

    /// <summary>
    /// Push all collected input from the last tick to the current one into the input buffer
    /// </summary>
    public void CollectInputs() => bufferHandler.PushBuffer();
    /// <summary>
    /// Reset all buffer inputs to default values
    /// </summary>
    public void ClearInputBuffer(bool keepActiveDirection = true) => bufferHandler.ClearBuffer(keepActiveDirection);

    /// <summary>
    /// Check all moves to see if input buffer correlates to one
    /// </summary>
    public bool TryReadAttack(out AttackData targetAttack)
    {
        int bestAttackStrength = 0;
        int attackStrength;
        targetAttack = new AttackData();

        int moveSetLength = moveSet.Length;
        for (int i = 0; i < moveSetLength; i++)
        {
            attackStrength = bufferHandler.TestInput(moveSet[i].Input);

            if (attackStrength <= bestAttackStrength)
                continue;

            bestAttackStrength = attackStrength;
            targetAttack = moveSet[i];

            // Perfect input found, no need to continue checking other moves in the moveset
            if (bestAttackStrength == 3)
                break;
        }

        return bestAttackStrength != 0;
    }

    /// <summary>
    /// Check if there is a sidestep input in the buffer, if so return it as 
    /// </summary>
    /// <returns></returns>
    public bool TryReadSideStep(out bool isSideStepUp) => bufferHandler.TestSideStep(out isSideStepUp);
    /// <summary>
    /// Get latest direction input from buffer
    /// </summary>
    public DirectionInput GetCurrentDirection() => bufferHandler.GetCurrentDirection();

    #endregion
}

[System.Serializable]
public class InputBufferHandler
{
    [EditorReadOnly, SerializeField] private FrameInput[] inputBuffer = new FrameInput[GlobalGameData.INPUT_BUFFER_SIZE];
    private int index;

    [EditorReadOnly, SerializeField] private FrameInput cRawInput;

    public DirectionInput GetCurrentDirection() => cRawInput.DirectionFlag;


    #region Buffer Update/Managament

    /// <summary>
    /// Add <paramref name="flag"/> to current tick's buffered attack button inputs
    /// </summary>
    public void UpdateCurrentInput(AttackInputFlags flag)
    {
        cRawInput.AttackFlags |= flag;
    }
    /// <summary>
    /// Set <paramref name="dir"/> in current tick's buffered direction
    /// </summary>
    public void UpdateCurrentDirection(DirectionInput dir)
    {
        cRawInput.DirectionFlag = dir;
    }

    /// <summary>
    /// Write collected input to buffer
    /// </summary>
    public void PushBuffer()
    {
        inputBuffer[index] = cRawInput;
        cRawInput.AttackFlags = AttackInputFlags.None;

        index.IncrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
    }
    /// <summary>
    /// Reset all buffer inputs to default values
    /// </summary>
    public void ClearBuffer(bool keepActiveDirection = true)
    {
        for (int i = 0; i < GlobalGameData.INPUT_BUFFER_SIZE; i++)
        {
            inputBuffer[i] = new FrameInput();
        }
        if (keepActiveDirection)
        {
            cRawInput.AttackFlags = AttackInputFlags.None;
        }
        else
        {
            cRawInput = new FrameInput();
        }
    }

    #endregion


    /// <summary>
    /// Check if input is found in buffer for inputted move their Keybinds (FrameInput)
    /// </summary>
    public int TestInput(FrameInput targetInput)
    {
        int moveStrength = 0;
        int bufferIndex = index;
        bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);

        // Loop over whole buffer starting from current index all the way back to it
        for (int i = 0; i < GlobalGameData.INPUT_BUFFER_SIZE; i++)
        {
            // Check if buffered input contains the same attack buttons, if not > next buffer input
            if (!inputBuffer[bufferIndex].AttackFlags.HasFlag(targetInput.AttackFlags))
            {
                bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
                continue;
            }

            // If button is correct and the to test inputs direction is neutral > award 1/3 points
            if (targetInput.DirectionFlag == DirectionInput.Neutral)
            {
                moveStrength = 1;
            }

            int dirIndex = bufferIndex;
            for (int j = 0; j < 1 + GlobalGameData.DIRECTION_BUFFER_WINDOW; j++)
            {
                // Check into the past of the buffer for X frames for if the target attacks direction is found
                if (inputBuffer[dirIndex].DirectionFlag == targetInput.DirectionFlag)
                {
                    // Exact move was matched with buffer history > award 3/3 points
                    return 3;
                }
                dirIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
            }

            bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
        }
        return moveStrength;
    }

    public bool TestSideStep(out bool isSideStepUp)
    {
        int dirHeldTickCount = 0;
        DirectionInput activeDir = DirectionInput.Neutral;

        int bufferIndex = index;
        if (inputBuffer[bufferIndex].DirectionFlag != DirectionInput.Neutral)
        {
            isSideStepUp = false;
            return false;
        }

        // Loop over whole buffer starting from current index all the way back to it
        for (int i = 0; i < GlobalGameData.INPUT_BUFFER_SIZE; i++)
        {
            DirectionInput currentDir = inputBuffer[bufferIndex].DirectionFlag;

            if (currentDir == DirectionInput.Up || currentDir == DirectionInput.Down)
            {
                // If dir changed compared to previous checked tick, reset dirHeldTickCount to 0
                if (activeDir != currentDir)
                {
                    dirHeldTickCount = 0;
                    activeDir = currentDir;
                }

                dirHeldTickCount += 1;
            }
            else if (currentDir == DirectionInput.Neutral)
            {
                // Check if dir was held long enough, but not too long for it to be considered a sidestep
                if (dirHeldTickCount > 0 && dirHeldTickCount <= GlobalGameData.SIDE_STEP_MAX_HOLD_TICKS)
                {
                    isSideStepUp = activeDir == DirectionInput.Up;
                    return true;
                }

                dirHeldTickCount = 0;
                activeDir = DirectionInput.Neutral;
            }

            bufferIndex.IncrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
        }

        isSideStepUp = false;
        return false;
    }
}