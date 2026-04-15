using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


namespace Fire_Pixel.Utility
{
#pragma warning disable UDR0002
#pragma warning disable UDR0004
    /// <summary>
    /// Uitlity class to have an optimized easy access to varying callbacks by using an Action based callback system
    /// Handles callbacks and batch them for every script by an event based register system
    /// </summary>
    public static class CallbackScheduler
    {
#pragma warning disable IDE1006
        private static event Action Update;
        private static event Action FrameTick;

        private static event Action LateApplicationQuit;
#pragma warning restore IDE1006

        private static readonly List<DelayedCallback> delayedCallbacks = new List<DelayedCallback>();

        private static bool quitting;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            CallbackRunnerInstance gameManager = new GameObject(">>UpdateScheduler<<").AddComponent<CallbackRunnerInstance>();
            gameManager.Init();

            GameObject.DontDestroyOnLoad(gameManager.gameObject);
        }


        #region void Update

        /// <summary>
        /// Register a method to call every frame like Update()
        /// </summary>
        public static void RegisterUpdate(Action action)
        {
            Update += action;
        }
        /// <summary>
        /// Unregister a registerd method for Update()
        /// </summary>
        public static void UnRegisterUpdate(Action action)
        {
            Update -= action;
        }
        /// <summary>
        /// Register or Unregister a method for Update() based on bool <paramref name="register"/>
        /// </summary>
        public static void ManageUpdate(Action action, bool register)
        {
            if (register)
            {
                RegisterUpdate(action);
            }
            else
            {
                UnRegisterUpdate(action);
            }
        }

        #endregion


        #region void FrameTick

        /// <summary>
        /// Register a method to call every frame like FrameTick()
        /// </summary>
        public static void RegisterFrameTick(Action action)
        {
            FrameTick += action;
        }
        /// <summary>
        /// Unregister a registerd method for FrameTick()
        /// </summary>
        public static void UnRegisterFrameTick(Action action)
        {
            FrameTick -= action;
        }
        /// <summary>
        /// Register or Unregister a method for FrameTick() based on bool <paramref name="register"/>
        /// </summary>
        public static void ManageFrameTick(Action action, bool register)
        {
            if (register)
            {
                RegisterFrameTick(action);
            }
            else
            {
                UnRegisterFrameTick(action);
            }
        }

        #endregion

        
        public static void CreateLateApplicationQuitCallback(Action action)
        {
            LateApplicationQuit += action;
        }


        #region Delayed Invoke Callbacks

        public static InvokeCallbackReference Invoke(float delay, Action callback, int groupId = 0)
        {
            int callbackId = delayedCallbacks.Count;
            DelayedCallback delayedCallback = new DelayedCallback(callback, Time.time + delay, groupId, callbackId);

            delayedCallbacks.Add(delayedCallback);

            return delayedCallback.CallbackRef;
        }
        /// <summary>
        /// Stops a previously scheduled Invoke Callback by ref and clears its reference.
        /// </summary>
        public static void CancelInvoke(InvokeCallbackReference callbackRef)
        {
            RemoveDelayedCallback(callbackRef.Id);
        }
        /// <summary>
        /// Cancel all invokes with the same group id, useful to cancel all callbacks of a script for example when it gets destroyed without having to save every callback reference
        /// </summary>
        public static void CancelAllInvokesInGroup(int groupId)
        {
            for (int i = delayedCallbacks.Count - 1; i >= 0; i--)
            {
                if (delayedCallbacks[i].GroupId == groupId)
                {
                    RemoveDelayedCallback(i);
                }
            }
        }

        /// <summary>
        /// Remove delayed callback and its reference by id
        /// </summary>
        private static void RemoveDelayedCallback(int toRemoveId)
        {
            delayedCallbacks[toRemoveId].CallbackRef = null;

            // If the callback to remove is not the last one, Update the last callback in list id to match new position after SwapBack
            if (toRemoveId != delayedCallbacks.Count - 1)
            {
                // Update the reference of the moved callback
                delayedCallbacks[^1].CallbackRef.Id = toRemoveId;
            }
            // Remove the callback and its reference
            delayedCallbacks.RemoveAtSwapBack(toRemoveId);
        }

        #endregion


        /// <summary>
        /// Callback runner instance to invoke the registered callbacks.
        /// </summary>
        private class CallbackRunnerInstance : MonoBehaviour
        {
            public static CallbackRunnerInstance Instance { get; set; }

            private float frameTimeAccumulator = 0f;

            private int cCatchUpTicks = 0;
            private const int MAX_CATCH_UP_TICKS = 5;


            public void Init()
            {
                Instance = this;
            }

            private void Update()
            {
                if (quitting)
                {
                    LateApplicationQuit?.Invoke();
                    LateApplicationQuit = null;
                    StopAllCoroutines();
                    return;
                }

                // Update
                CallbackScheduler.Update?.Invoke();

                // Frame Tick (Up to 5 times per frame for catchup)
                cCatchUpTicks = 0;
                frameTimeAccumulator += Time.unscaledDeltaTime;

                while (frameTimeAccumulator >= GlobalGameData.TICK_TIME)
                {
                    FrameTick?.Invoke();
                    frameTimeAccumulator -= GlobalGameData.TICK_TIME;

                    cCatchUpTicks += 1;
                    if (cCatchUpTicks == MAX_CATCH_UP_TICKS) break;
                }

                // Invoke delayed callbacks
                float time = Time.time;
                for (int i = delayedCallbacks.Count - 1; i >= 0; i--)
                {
                    if (time >= delayedCallbacks[i].InvokeGlobalTime)
                    {
                        Action callback = delayedCallbacks[i].Callback;
                        callback?.Invoke();
                        RemoveDelayedCallback(i);
                    }
                }
            }

            private void OnApplicationQuit()
            {
                quitting = true;
            }
            private void OnDestroy()
            {
                CallbackScheduler.Update = null;
                CallbackScheduler.LateApplicationQuit = null;
            }
        }
    }
#pragma warning restore UDR0002
#pragma warning restore UDR0004
}

[System.Serializable]
public class DelayedCallback
{
    public Action Callback;
    public float InvokeGlobalTime;
    public int GroupId;
    public InvokeCallbackReference CallbackRef;

    public DelayedCallback(Action callback, float invokeGlobalTime, int groupId, int callbackId)
    {
        Callback = callback;
        InvokeGlobalTime = invokeGlobalTime;
        GroupId = groupId;
        CallbackRef = new InvokeCallbackReference(callbackId);
    }
}
[System.Serializable]
public class InvokeCallbackReference
{
    public int Id;

    public InvokeCallbackReference(int id)
    {
        Id = id;
    }
}