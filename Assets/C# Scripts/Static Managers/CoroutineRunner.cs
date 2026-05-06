using UnityEngine;


namespace Fire_Pixel.Utility
{
    /// <summary>
    /// Static class that generates a singleton MB instance that can schedules coroutines through this class.
    /// </summary>
    public static class CoroutineRunner
    {
        public static CoroutineRunnerInstance Instance { get; private set; }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init()
        {
            GameObject go = new GameObject(">>CoroutineRunner<<");
            Instance = go.AddComponent<CoroutineRunnerInstance>();
            GameObject.DontDestroyOnLoad(go);
        }
        public class CoroutineRunnerInstance : MonoBehaviour { }
    }
}