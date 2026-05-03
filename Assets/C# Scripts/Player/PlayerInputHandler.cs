using Unity.Collections;
using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for handling player moves 
/// </summary>
[System.Serializable]
public class PlayerInputHandler
{
    [ReadOnly, SerializeField] private AttackData[] moveSet;
    [EditorReadOnly, SerializeField] private InputBufferHandler bufferHandler;


    public PlayerInputHandler(AttackData[] moveSet)
    {
        this.moveSet = moveSet;
        bufferHandler = new InputBufferHandler();
    }

    
    #region Player Input Callbacks

    // Called by the PlayerInput component when an input is performed or canceled, with that corresponding button as AttackInputFlag
    public void OnButtonPressed(AttackInputFlags flag)
    {
        bufferHandler.UpdateCurrentInput(flag);
    }
    // Called by the PlayerInput component when the directional input is performed or canceled.
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


    /// <summary>
    /// Push all collected input from the last tick to the current one into the input buffer
    /// </summary>
    public void CollectInputs()
    {
        bufferHandler.PushBuffer();
    }

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
    /// Get latest direction input from buffer
    /// </summary>
    public DirectionInput GetCurrentDirection() => bufferHandler.GetCurrentDirection();
}

[System.Serializable]
public class InputBufferHandler
{
    [SerializeField] private FrameInput[] inputBuffer = new FrameInput[GlobalGameData.INPUT_BUFFER_SIZE];
    private int index;

    [SerializeField] private FrameInput cRawInput;

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

    public void PushBuffer()
    {
        // Write collected input to buffer
        inputBuffer[index] = cRawInput;
        cRawInput.AttackFlags = AttackInputFlags.None;

        index.IncrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
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
}