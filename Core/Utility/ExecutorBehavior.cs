﻿using System;
using Elarion.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion.Utility {
    public class ExecutorBehavior : MonoBehaviour {
        [SerializeField, ReadOnly]
        private bool _dontDestroyOnLoad;

        public event Action StartEvent = () => { };
        public event Action EnableEvent = () => { };
        public event Action DisableEvent = () => { };
        public event Action DestroyEvent = () => { };
        
        public event Action FixedUpdateEvent = () => { };
        public event Action UpdateEvent = () => { };
        public event Action LateUpdateEvent = () => { };


        public event Action ApplicationFocusEvent = () => { };
        public event Action ApplicationPauseEvent = () => { };
        public event Action ApplicationQuitEvent = () => { };
        
        public event Action DrawGizmosEvent = () => { };
        public event Action GUIEvent = () => { };
        public event Action PostRenderEvent = () => { };
        public event Action PreCullEvent = () => { };
        public event Action PreRenderEvent = () => { };
        public event Action ResetEvent = () => { };
        public event Action ValidateEvent = () => { };

        public new bool DontDestroyOnLoad {
            get { return _dontDestroyOnLoad; }
            set {
                if(value) {
                    Object.DontDestroyOnLoad(gameObject);
                }

                if(!value && _dontDestroyOnLoad) {
                    Debug.Log("Object already set to not destroy on load. This cannot be undone (you can destroy the object instead).");
                    return;                    
                }
                
                _dontDestroyOnLoad = value;
            }
        }
        
        #region Lifecycle Events
        
        private void Start() {
            StartEvent();
        }
        
        private void OnEnable() {
            EnableEvent();
        }

        private void OnDisable() {
            DisableEvent();
        }

        private void OnDestroy() {
            DestroyEvent();
        }

        #endregion
        
        #region UpdateEvents

        private void FixedUpdate() {
            FixedUpdateEvent();
        }
        
        private void Update() {
            UpdateEvent();
        }

        private void LateUpdate() {
            LateUpdateEvent();
        }

        #endregion

        #region Application Events

        private void OnApplicationFocus(bool hasFocus) {
            ApplicationFocusEvent();
        }

        private void OnApplicationPause(bool pauseStatus) {
            ApplicationPauseEvent();
        }

        private void OnApplicationQuit() {
            ApplicationQuitEvent();
        }

        #endregion

        #region Other Events

        private void OnDrawGizmos() {
            DrawGizmosEvent();
        }


        private void OnGUI() {
            GUIEvent();
        }

        private void OnPostRender() {
            PostRenderEvent();
        }

        private void OnPreCull() {
            PreCullEvent();
        }

        private void OnPreRender() {
            PreRenderEvent();
        }

        private void OnValidate() {
            ValidateEvent();
        }

        private void Reset() {
            ResetEvent();
        }

        #endregion

        public static TExecutor Create<TExecutor>(string name = "Executor", bool dontDestroyOnLoad = false, HideFlags hideFlags = HideFlags.None) where TExecutor : ExecutorBehavior {
            var go = new GameObject(name) {
                hideFlags = hideFlags
            };

            var executor = go.AddComponent<TExecutor>();

            executor.DontDestroyOnLoad = dontDestroyOnLoad;

            return executor;
        }
    }
}