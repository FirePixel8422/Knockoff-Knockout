using Unity.Mathematics;
using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for handling player moves 
/// </summary>
[System.Serializable]
public class PlayerInputHandler
{
    [SerializeField] private InputBufferHandler bufferHandler = new InputBufferHandler();

    private const float DEADZONE = GlobalGameData.STICK_DEADZONE;


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
        int2 dir = new int2()
        {
            x = dirVec.x > DEADZONE ? 1 : dirVec.x < -DEADZONE ? -1 : 0,
            y = dirVec.y > DEADZONE ? 1 : dirVec.y < -DEADZONE ? -1 : 0
        };
        bufferHandler.UpdateCurrentDirection(dir);
    }

    public void OnFrameTick() => bufferHandler.PushBuffer();
}

[System.Serializable]
public class InputBufferHandler
{
    [SerializeField] private FrameInput[] inputBuffer = new FrameInput[GlobalGameData.INPUT_BUFFER_SIZE];
    private int index;

    private FrameInput cRawInput;


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
    public void UpdateCurrentDirection(int2 dir)
    {
        cRawInput.Direction = dir;
    }

    public void PushBuffer()
    {
        // Write collected input to buffer
        inputBuffer[index] = cRawInput;

        index.IncrementSmart(GlobalGameData.INPUT_BUFFER_SIZE);
    }
}