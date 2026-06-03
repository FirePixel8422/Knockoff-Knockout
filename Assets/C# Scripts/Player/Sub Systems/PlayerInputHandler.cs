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

    #endregion
    

    #region Input Reading From InputBuffer

    /// <summary>
    /// Get latest direction input from buffer
    /// </summary>
    public DirectionInput GetCurrentDirection() => bufferHandler.GetCurrentDirection();

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
            attackStrength = bufferHandler.TestAttack(moveSet[i].Input);

            if (attackStrength <= bestAttackStrength)
                continue;

            bestAttackStrength = attackStrength;
            targetAttack = moveSet[i];

            // Perfect input found, no need to continue checking other moves in the moveset
            if (bestAttackStrength == 4)
                break;
        }

        return bestAttackStrength != 0;
    }

    /// <summary>
    /// Check if there is a sidestep input in the buffer, if so return true and dash direction as bool <paramref name="isSideStepUp"/>
    /// </summary>
    public bool TryReadSideStep(out bool isSideStepUp) => bufferHandler.TestSideStep(out isSideStepUp);

    /// <summary>
    /// Check if there is a dash input in the buffer, if so return true and dash direction as bool <paramref name="isDashForward"/>
    /// </summary>
    public bool TryReadDash(out bool isDashForward) => bufferHandler.TestDash(out isDashForward);

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


    #region Input Reading

    public int TestAttack(FrameInput targetInput)
    {
        int moveStrength = 0;
        int bufferIndex = index;
        bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);

        // Loop over whole buffer starting from current index all the way back to it
        for (int i = 0; i < GlobalGameData.ATTACK_BUFFER_SIZE; i++)
        {
            // Check if buffered input contains the same attack buttons, if not > next buffer input
            if ((inputBuffer[bufferIndex].AttackFlags & targetInput.AttackFlags) == 0)
            {
                bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
                continue;
            }

            // If button is correct and the to test inputs direction is neutral > award 1/4 points
            if (targetInput.DirectionFlag == DirectionInput.Neutral)
            {
                moveStrength = 1;
            }

            int dirIndex = bufferIndex;
            for (int i2 = 0; i2 < 1 + GlobalGameData.DIRECTION_BUFFER_WINDOW; i2++)
            {
                // Check into the past of the buffer for X frames for if the target attacks direction is found
                if (inputBuffer[dirIndex].DirectionFlag == targetInput.DirectionFlag)
                {
                    // Exact move was matched with buffer history > award 3/4 points
                    return targetInput.DirectionFlag == DirectionInput.Neutral ? 3 : 4;
                }
                dirIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
            }

            bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
        }
        return moveStrength;
    }

    public bool TestSideStep(out bool isSideStepUp)
    {
        DirectionInput firstTap = DirectionInput.Neutral;

        for (int i = 1; i <= GlobalGameData.MAX_DOUBLE_TAP_TICKS; i++)
        {
            int idx = (index - i + GlobalGameData.INPUT_BUFFER_SIZE) % GlobalGameData.INPUT_BUFFER_SIZE;
            int prevIdx = (idx - 1 + GlobalGameData.INPUT_BUFFER_SIZE) % GlobalGameData.INPUT_BUFFER_SIZE;

            DirectionInput current = inputBuffer[idx].DirectionFlag;
            DirectionInput prev = inputBuffer[prevIdx].DirectionFlag;

            bool isPress =
                (current == DirectionInput.Up || current == DirectionInput.Down) &&
                current != prev;

            if (!isPress)
                continue;

            if (firstTap == DirectionInput.Neutral)
            {
                firstTap = current;
                continue;
            }

            if (current == firstTap)
            {
                isSideStepUp = current == DirectionInput.Up;
                return true;
            }
        }

        isSideStepUp = false;
        return false;
    }

    public bool TestDash(out bool isDashRight)
    {
        DirectionInput firstTap = DirectionInput.Neutral;

        for (int i = 1; i <= GlobalGameData.MAX_DOUBLE_TAP_TICKS; i++)
        {
            int idx = (index - i + GlobalGameData.INPUT_BUFFER_SIZE) % GlobalGameData.INPUT_BUFFER_SIZE;
            int prevIdx = (idx - 1 + GlobalGameData.INPUT_BUFFER_SIZE) % GlobalGameData.INPUT_BUFFER_SIZE;

            DirectionInput current = inputBuffer[idx].DirectionFlag;
            DirectionInput prev = inputBuffer[prevIdx].DirectionFlag;

            bool isPress =
                (current == DirectionInput.Left || current == DirectionInput.Right) &&
                current != prev;

            if (!isPress)
                continue;

            if (firstTap == DirectionInput.Neutral)
            {
                firstTap = current;
                continue;
            }

            if (current == firstTap)
            {
                isDashRight = current == DirectionInput.Right;
                return true;
            }
        }

        isDashRight = false;
        return false;
    }

    #endregion
}