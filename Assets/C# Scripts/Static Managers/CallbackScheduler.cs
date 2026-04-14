using System;
using System.Collections;
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
        private static event Action Update;
        private static event Action LateUpdate;
        private static event Action FixedUpdate;

        private static event Action LateDestroy;
        private static event Action LateApplicationQuit;

        private static readonly List<DelayedCallback> delayedCallbacks = new List<DelayedCallback>();
        private static readonly List<InvokeCallbackReference> callbackReferences = new List<InvokeCallbackReference>();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            CallbackRunnerInstance gameManager = new GameObject(">>CallbackScheduler<<").AddComponent<CallbackRunnerInstance>();
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


        #region void LateUpdate

        /// <summary>
        /// Register a method to call after every frame like LateUpdate()
        /// </summary>
        public static void RegisterLateUpdate(Action action)
        {
            LateUpdate += action;
        }
        /// <summary>
        /// Unregister a registerd method for LateUpdate()
        /// </summary>
        public static void UnRegisterLateUpdate(Action action)
        {
            LateUpdate -= action;
        }
        /// <summary>
        /// Register or Unregister a method for LateUpdate() based on bool <paramref name="register"/>
        /// </summary>
        public static void ManageLateUpdate(Action action, bool register)
        {
            if (register)
            {
                RegisterLateUpdate(action);
            }
            else
            {
                UnRegisterLateUpdate(action);
            }
        }

        #endregion


        #region void FixedUpdate

        /// <summary>
        /// Register a method to call every fixed frame like FixedUpdate()
        /// </summary>
        public static void RegisterFixedUpdate(Action action)
        {
            FixedUpdate += action;
        }
        /// <summary>
        /// Unregister a registerd method for FixedUpdate()
        /// </summary>
        public static void UnRegisterFixedUpdate(Action action)
        {
            FixedUpdate -= action;
        }
        /// <summary>
        /// Register or Unregister a method for FixedUpdate() based on bool <paramref name="register"/>
        /// </summary>
        public static void ManageFixedUpdate(Action action, bool register)
        {
            if (register)
            {
                RegisterFixedUpdate(action);
            }
            else
            {
                UnRegisterFixedUpdate(action);
            }
        }

        #endregion


        public static void CreateLateDestroyCallback(Action action)
        {
            LateDestroy += action;
        }
        public static void CreateLateApplicationQuitCallback(Action action)
        {
            LateApplicationQuit += action;
        }


        #region Delayed Invoke Callbacks

        public static InvokeCallbackReference Invoke(float delay, Action callback, int groupId = 0)
        {
            delayedCallbacks.Add(new DelayedCallback(callback, Time.time + delay, groupId));

            InvokeCallbackReference callbackRef = new InvokeCallbackReference(delayedCallbacks.Count - 1);
            callbackReferences.Add(callbackRef);

            return callbackRef;
        }
        /// <summary>
        /// Stops a previously scheduled Invoke Callback by ref and clears its reference.
        /// </summary>
        public static void CancelInvoke(ref InvokeCallbackReference callbackRef)
        {
            if (callbackRef == null) return;

            RemoveDelayedCallback(callbackRef.Id);

            // Destroy callback reference
            callbackRef = null;
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
            // If the callback to remove is not the last one, update the last callback in list id to match new position after SwapBack
            if (toRemoveId != delayedCallbacks.Count - 1)
            {
                // Update the reference of the moved callback
                callbackReferences[^1].SetId(toRemoveId);
            }
            // Remove the callback and its reference
            callbackReferences.RemoveAtSwapBack(toRemoveId);
            delayedCallbacks.RemoveAtSwapBack(toRemoveId);
        }


        [System.Serializable]
        public struct DelayedCallback
        {
            public Action Callback;
            public float InvokeGlobalTime;
            public int GroupId;

            public DelayedCallback(Action callback, float invokeGlobalTime, int groupId)
            {
                Callback = callback;
                InvokeGlobalTime = invokeGlobalTime;
                GroupId = groupId;
            }
        }

        #endregion


        /// <summary>
        /// Callback runner instance to invoke the registered callbacks.
        /// </summary>
        private class CallbackRunnerInstance : MonoBehaviour
        {
            public static CallbackRunnerInstance Instance { get; set; }

            public void Init()
            {
                Instance = this;
                StartCoroutine(UpdateLoop());
            }
            private IEnumerator UpdateLoop()
            {
                float fixedAccumulator = 0f;
                float fixedDelta = Time.fixedDeltaTime;

                while (true)
                {
                    // Update
                    Update?.Invoke();

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

                    // FixedUpdate
                    fixedAccumulator += Time.deltaTime;
                    while (fixedAccumulator >= fixedDelta)
                    {
                        FixedUpdate?.Invoke();
                        fixedAccumulator -= fixedDelta;
                    }

                    // LateUpdate
                    LateUpdate?.Invoke();

                    if (LateDestroy != null)
                    {
                        LateDestroy.Invoke();
                        LateDestroy = null;
                    }

                    yield return null;
                }
            }

            private void OnApplicationQuit()
            {
                LateUpdate += () =>
                {
                    if (LateApplicationQuit != null)
                    {
                        LateApplicationQuit.Invoke();
                        LateApplicationQuit = null;
                        StopAllCoroutines();
                    }
                };
            }
            private void OnDestroy()
            {
                Update = null;
                LateUpdate = null;
                FixedUpdate = null;

                LateDestroy = null;
                LateApplicationQuit = null;

                StopAllCoroutines();
            }
        }
    }
#pragma warning restore UDR0002
#pragma warning restore UDR0004
}

[System.Serializable]
public class InvokeCallbackReference
{
    public int Id { get; private set; }
    public void SetId(int id) => Id = id;

    public InvokeCallbackReference(int id)
    {
        Id = id;
    }
}