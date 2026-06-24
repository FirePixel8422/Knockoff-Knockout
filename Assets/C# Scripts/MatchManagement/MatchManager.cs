using System;
using UnityEngine;


public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    [field: SerializeField] public GameState GameState { get; private set; }
    public void SetGameState(GameState newState) => GameState = newState;

    [field: SerializeField] public bool IsGamePaused { get; private set; }
    public bool IsMatchRestarting;

    public event Action OnGamePaused;
    public event Action OnGameUnPaused;


    private void Awake() => Instance = this;
    private void OnDestroy()
    {
        Time.timeScale = 1;

        OnGamePaused = null;
        OnGameUnPaused = null;
    }

    public void StartMatch()
    {
        IsMatchRestarting = true;

        SetGameState(GameState.InGame);

        HUDManager.Instance.ResetHUD();
        PlayerManager.Instance.ResetPlayers();
        HUDManager.Instance.ResetHUD();

        MatchStartAnimator.Instance.StartTimer();

        IsMatchRestarting = false;
    }
    public void EndMatch()
    {
        SetGameState(GameState.PreGame);
    }

    public void PauseGame()
    {
        OnGamePaused?.Invoke();
        IsGamePaused = true;
        Time.timeScale = 0;
    }
    public void UnPauseGame()
    {
        OnGameUnPaused?.Invoke();
        IsGamePaused = false;
        Time.timeScale = 1;
    }
}