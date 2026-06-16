using System;
using UnityEngine;


public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }
    private void Awake() => Instance = this;


    [SerializeField] private bool gamePaused;
    public bool GamePaused => gamePaused;
    public event Action OnGamePaused;
    public event Action OnGameUnPaused;

    public void PauseGame()
    {
        OnGamePaused?.Invoke();
        gamePaused = true;
        Time.timeScale = 0;
    }
    public void UnPauseGame()
    {
        OnGameUnPaused?.Invoke();
        gamePaused = false;
        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;

        OnGamePaused = null;
        OnGameUnPaused = null;
    }
}