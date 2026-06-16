using System.Diagnostics;
using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for handling player moves 
/// </summary>
[System.Serializable]
public class PlayerInputHandler
{
    [EditorReadOnly, SerializeField] private AttackData[] moveSet;
    [EditorReadOnly, SerializeField] private AttackData[] stringSet;
    [EditorReadOnly, SerializeField] private InputBufferHandler bufferHandler;

    private readonly bool isLeftPlayer;

    public PlayerInputHandler(AttackData[] moveSet, AttackData[] stringSet, bool isLeftPlayer)
    {
        this.moveSet = moveSet;
        this.stringSet = stringSet;
        this.isLeftPlayer = isLeftPlayer;

        bufferHandler = new InputBufferHandler();
    }
    private PlayerInputHandler() { }
    public void Dispose()
    {
        int attackCount = moveSet.Length;
        for (int i = 0; i < attackCount; i++)
        {
            moveSet[i].Dispose();
        }
    }

    
    #region Player Input/Overrider Callbacks

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
    /// <summary>
    /// Override <paramref name="frameInput"/> in input buffer
    /// </summary>
    public void OnInputOverride(FrameInput frameInput)
    {
        bufferHandler.UpdateCurrentInput(frameInput);
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
            attackStrength = bufferHandler.TestAttack(moveSet[i].Input, isLeftPlayer);

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
    /// Check all string moves to see if input buffer correlates to one
    /// </summary>
    public bool TryReadStringAttack(StringTransition[] stringOptions, out AttackData targetAttack)
    {
        targetAttack = new AttackData();

        int bestAttackStrength = 0;
        int attackStrength;

        int stringOptionsCount = stringOptions.Length;
        for (int i = 0; i < stringOptionsCount; i++)
        {
            int stringAttackId = stringOptions[i].TargetMoveId;

            attackStrength = bufferHandler.TestAttack(stringSet[stringAttackId].Input, isLeftPlayer);

            if (attackStrength <= bestAttackStrength)
                continue;

            bestAttackStrength = attackStrength;

            targetAttack = stringSet[stringAttackId];

            // Perfect input found, no need to continue checking other moves in the moveset
            if (bestAttackStrength == 3)
                break;
        }

        return bestAttackStrength != 0;
    }

    /// <summary>
    /// Check if there is a sidestep input in the buffer, if so return true and sidestep direction as bool <paramref name="isSideStepUp"/>
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


    #region Input Writing

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
    /// Override <paramref name="frameInput"/> in current tick's buffered frameInput
    /// </summary>
    public void UpdateCurrentInput(FrameInput frameInput)
    {
        cRawInput = frameInput;
    }

    #endregion


    #region Buffer Update/Managament

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

    /// <summary>
    /// Check if attack input '<paramref name="targetInput"/>' is in the input buffer. If so, return 
    /// </summary>
    public int TestAttack(FrameInput targetInput, bool isLeftPlayer)
    {
        int moveStrength = 0;
        int bufferIndex = index;
        bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);

        DirectionInput targetDirection = targetInput.DirectionFlag switch
        {
            DirectionInput.Left => isLeftPlayer ? DirectionInput.Left : DirectionInput.Right,
            DirectionInput.Right => isLeftPlayer ? DirectionInput.Right : DirectionInput.Left,
            _ => targetInput.DirectionFlag
        };
        

        // Loop over whole buffer starting from current index all the way back to it
        for (int i = 0; i < GlobalGameData.ATTACK_BUFFER_SIZE; i++)
        {
            // Check if buffered input contains the same attack buttons, if not > next buffer input
            if ((inputBuffer[bufferIndex].AttackFlags & targetInput.AttackFlags) == 0)
            {
                bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
                continue;
            }

            // If button is correct and targetInput direction is neutral > award 1/3 points
            if (targetDirection == DirectionInput.Neutral)
            {
                moveStrength = 1;
            }

            int dirIndex = bufferIndex;
            for (int i2 = 0; i2 < 1 + GlobalGameData.DIRECTION_BUFFER_WINDOW; i2++)
            {
                // Check into the past of the buffer for X frames for if the target attacks direction is found
                if (inputBuffer[dirIndex].DirectionFlag == targetDirection)
                {
                    // Exact move was matched with buffer history >
                    // If move is neutral direction > award 2/3 points
                    // If move is non neutral direction > award 3/3 points
                    // This so directional moves take execution priority over neutral moves.
                    return targetDirection == DirectionInput.Neutral ? 2 : 3;
                }
                dirIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
            }

            bufferIndex.DecrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
        }
        return moveStrength;
    }

    /// <summary>
    /// Check if there is a dash input in the input buffer. (If a directional double tap (Down/Up) taps are within the <see cref="GlobalGameData.MAX_DOUBLE_TAP_TICKS"/> window)
    /// </summary>
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

    /// <summary>
    /// Check if there is a dash input in the input buffer. (If a directional double tap (Left/Right) taps are within the <see cref="GlobalGameData.MAX_DOUBLE_TAP_TICKS"/> window)
    /// </summary>
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