using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerAssigner : MonoBehaviour
{
    public static ControllerAssigner Instance { get; private set; }


    [SerializeField] private RectTransform[] controllers;

    private readonly Vector2[] prevDirVecs = new Vector2[GlobalGameData.MAX_PLAYERS];
    private readonly bool[] freeIds = new bool[GlobalGameData.MAX_PLAYERS];

    private readonly Dictionary<PlayerInput, int> playerToId = new();

    private void Awake()
    {
        Instance = this;
        Array.Fill(freeIds, true);
    }

    public void AssignInputDevice(PlayerInput playerInput)
    {
        int freeId = -1;
        for (int i = 0; i < freeIds.Length; i++)
        {
            if (freeIds[i] == true)
            {
                freeId = i;
                freeIds[i] = false;
                break;
            }
        }

        if (freeId == -1)
        {
            DebugLogger.LogError("No free id found for " + playerInput.name);
            return;
        }

        playerToId[playerInput] = freeId;

        playerInput.actions["Direction"].performed += ctx => OnDirection(ctx, freeId);
        playerInput.actions["Direction"].canceled += ctx => OnDirection(ctx, freeId);
    }
    public void RemoveInputDevice(PlayerInput playerInput)
    {
        if (playerToId.TryGetValue(playerInput, out int freeId))
        {
            freeIds[freeId] = true;
            playerToId.Remove(playerInput);
        }
    }
    private void OnDirection(InputAction.CallbackContext ctx, int id)
    {
        Vector2 vecDir = ctx.ReadValue<Vector2>();

        Vector2 prev = prevDirVecs[id];

        bool wasLeft = prev.x < -0.5f;
        bool wasRight = prev.x > 0.5f;

        bool isLeft = vecDir.x < -0.5f;
        bool isRight = vecDir.x > 0.5f;

        // TAP LEFT
        if (isLeft && !wasLeft)
        {
            Debug.Log("Tap Left " + id);
        }

        // TAP RIGHT
        if (isRight && !wasRight)
        {
            Debug.Log("Tap Right " + id);
        }

        prevDirVecs[id] = vecDir;
    }
}