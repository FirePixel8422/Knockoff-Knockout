using UnityEngine;


public class PlayerQuit : MonoBehaviour
{
    /// <summary>
    /// Quits the game or stops play mode in the editor.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
