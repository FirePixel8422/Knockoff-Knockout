using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ControllerAssigner : UpdateMonoBehaviour
{
    public static ControllerAssigner Instance { get; private set; }


    [SerializeField] private GameObject uiRoot;

    [SerializeField] private Image[] controllerImages;
    [SerializeField] private float[] controllerSlotPositions;
    [SerializeField] private int[] fighterSlotToFighterId;

    [SerializeField] private float animationLerp;
    [SerializeField] private GamepadRumbleParameters onSelectPlayerRumble = GamepadRumbleParameters.ShortSoftRumble;

    private readonly Vector2[] prevDirInputs = new Vector2[GlobalGameData.MAX_PLAYERS];
    private readonly bool[] usedPlayerIds = new bool[GlobalGameData.MAX_PLAYERS];
    private readonly int[] playerFighterSlotIds = new int[GlobalGameData.MAX_PLAYERS];

    private readonly Dictionary<InputDevice, int> deviceToIdMap = new(GlobalGameData.MAX_PLAYERS);

    private const int UNASSIGNED_FIGHTER_SLOT_ID = GlobalGameData.MAX_PLAYERS / 2;

    private void Awake()
    {
        Instance = this;
        Array.Fill(playerFighterSlotIds, UNASSIGNED_FIGHTER_SLOT_ID);
    }


    #region Start/End ControllerAssignment

    public void StartControllerAssignment()
    {
        MatchManager.Instance.PauseGame();
        PlayerManager.Instance.UnbindAllPlayerInput();
        uiRoot.SetActive(true);

        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            controllerImages[i].gameObject.SetActiveSmart(usedPlayerIds[i]);
        }
    }
    public void EndControllerAssignment()
    {
        MatchManager.Instance.UnPauseGame();
        uiRoot.SetActive(false);
    }

    #endregion


    #region Assign and Remove Input Devices

    public void AssignInputDevice(PlayerInput playerInput)
    {
        int freeId = -1;
        for (int i = 0; i < usedPlayerIds.Length; i++)
        {
            if (usedPlayerIds[i] == false)
            {
                freeId = i;
                usedPlayerIds[i] = true;
                break;
            }
        }
        if (freeId == -1)
        {
            DebugLogger.LogError("No free id found for " + playerInput.name);
            return;
        }

        DebugLogger.Log("Assigned Input Device " + playerInput.name, PlayerManager.Instance.LogInputDeviceChanges);

        deviceToIdMap[playerInput.devices[0]] = freeId;

        playerInput.actions["Direction"].performed += OnDirection;
        playerInput.actions["Direction"].canceled += OnDirection;
        playerInput.actions["Start"].performed += OnStart;

        playerInput.SwitchCurrentActionMap("Gameplay");
        playerInput.actions.FindActionMap("Misc").Enable();

        UpdateAllowPlayerJoinState();
        StartControllerAssignment();
    }
    public void RemoveInputDevice(PlayerInput playerInput)
    {
        InputDevice device = playerInput.devices[0];

        DebugLogger.Log("Removed Input Device " + playerInput.name, PlayerManager.Instance.LogInputDeviceChanges);

        if (deviceToIdMap.TryGetValue(device, out int targetId))
        {
            usedPlayerIds[targetId] = false;
            playerFighterSlotIds[targetId] = UNASSIGNED_FIGHTER_SLOT_ID;
            deviceToIdMap.Remove(device);

            playerInput.actions["Direction"].performed -= OnDirection;
            playerInput.actions["Direction"].canceled -= OnDirection;
            playerInput.actions["Start"].performed -= OnStart;

            UpdateAllowPlayerJoinState();

            Vector3 unassignedPos = controllerImages[targetId].rectTransform.localPosition;
            unassignedPos.x = 0;

            UpdateControllerImage(controllerImages[targetId], unassignedPos, Color.white);
        }
    }

    private void UpdateAllowPlayerJoinState()
    {
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            if (usedPlayerIds[i] == false)
            {
                if (PlayerInputManager.instance.joiningEnabled == false)
                {
                    PlayerInputManager.instance.EnableJoining();
                }
                return;
            }
        }
        if (PlayerInputManager.instance.joiningEnabled)
        {
            PlayerInputManager.instance.DisableJoining();
        }
    }

    #endregion


    #region Input Callbacks

    private void OnDirection(InputAction.CallbackContext ctx)
    {
        if (!uiRoot.activeInHierarchy) return;

        if (!deviceToIdMap.TryGetValue(ctx.control.device, out int playerId))
        {   
            DebugLogger.LogError("Player id not found for device " + ctx.control.device.name);
            return;
        }

        Vector2 vecDir = ctx.ReadValue<Vector2>();
        int xDirection = CalculateHorizontalTap(vecDir, playerId);

        if (xDirection == 0) return;

        MoveFighterSlot(playerId, xDirection, ctx.control.device);
    }

    private void OnStart(InputAction.CallbackContext ctx)
    {
        InputDevice device = ctx.control.device;
        if (!deviceToIdMap.ContainsKey(device))
        {
            DebugLogger.LogError("Player id not found for device " + device.name);
            return;
        }

        if (uiRoot.activeInHierarchy)
        {
            foreach (var kvp in deviceToIdMap)
            {
                int deviceId = kvp.Value;
                PlayerInput playerInput = PlayerInput.GetPlayerByIndex(deviceId);

                if (!playerInput.TryGetComponent(out PlayerInputBinder binder))
                {
                    DebugLogger.LogError("PlayerInputBinder not found for player " + deviceId, PlayerManager.Instance.LogInputDeviceChanges);
                    return;
                }

                int fighterId = playerFighterSlotIds[deviceId];
                if (fighterId != UNASSIGNED_FIGHTER_SLOT_ID)
                {
                    PlayerManager.Instance.BindPlayerInput(binder, fighterSlotToFighterId[fighterId]);
                }
            }
            EndControllerAssignment();
        }
        else
        {
            StartControllerAssignment();
        }
    }

    #endregion


    #region Controller To Player Assignment

    /// <summary>
    /// Get horizontal stick tap movement as normalized int direction (-1, 0, 1).
    /// </summary>
    private int CalculateHorizontalTap(Vector2 vecDir, int playerId)
    {
        Vector2 prev = prevDirInputs[playerId];
        prevDirInputs[playerId] = vecDir;

        bool wasLeft = prev.x < -0.5f;
        bool wasRight = prev.x > 0.5f;

        bool isLeft = vecDir.x < -0.5f;
        bool isRight = vecDir.x > 0.5f;

        if (isLeft && !wasLeft)
        {
            return -1;
        }
        if (isRight && !wasRight)
        {
            return 1;
        }
        return 0;
    }
    /// <summary>
    /// Try to move into free fighter slot based on their direction input, also try to do gamepad rumble
    /// </summary>
    private void MoveFighterSlot(int playerId, int direction, InputDevice device)
    {
        int currentSlotId = playerFighterSlotIds[playerId];
        int newSlotId = currentSlotId;

        int max = GlobalGameData.MAX_PLAYERS;

        while (true)
        {
            newSlotId += direction;

            if (newSlotId < 0 || newSlotId > max)
                return;

            if (newSlotId == UNASSIGNED_FIGHTER_SLOT_ID || !playerFighterSlotIds.Contains(newSlotId))
                break;
        }

        playerFighterSlotIds[playerId] = newSlotId;
        if (device is Gamepad pad)
        {
            GamepadRumble.SetRumble(pad, onSelectPlayerRumble);
        }
        else
        {
            DebugLogger.Log("nah");
        }
    }

    #endregion


    protected override void OnUpdate()
    {
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            Vector3 currentPos = controllerImages[i].rectTransform.localPosition;
            Vector3 targetPos = new Vector3(controllerSlotPositions[playerFighterSlotIds[i]], currentPos.y, currentPos.z);

            int playerId = fighterSlotToFighterId[playerFighterSlotIds[i]];
            Color currentColor = controllerImages[i].color;
            Color targetColor = playerId == -1 ? Color.white : PlayerManager.Instance.PlayerColors[playerId];

            float t = animationLerp * Time.unscaledDeltaTime;

            UpdateControllerImage(controllerImages[i], 
                Vector3.Lerp(currentPos, targetPos, t), 
                Color.Lerp(currentColor, targetColor, t));
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateControllerImage(Image target, Vector3 pos, Color color)
    {
        target.rectTransform.localPosition = pos;
        target.color = color;
    }


    private void OnDestroy() => TryCleanupInputEvents();
    private void TryCleanupInputEvents()
    {
        foreach (var kvp in deviceToIdMap)
        {
            PlayerInput playerInput = PlayerInput.GetPlayerByIndex(kvp.Value);
            if (playerInput == null)
            {
#if UNITY_EDITOR
                cleanupFails += 1;
                DebugLogger.LogError("Critical Memmory Error, Events not cleaned up...", cleanupFails == 2);
#endif
                return;
            }

            playerInput.actions["Direction"].performed -= OnDirection;
            playerInput.actions["Direction"].canceled -= OnDirection;
            playerInput.actions["Start"].performed -= OnStart;
        }
    }

#if UNITY_EDITOR
    private int cleanupFails;

    private void OnApplicationQuit() => TryCleanupInputEvents();
#endif
}