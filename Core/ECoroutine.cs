using System;
using System.Collections;
using UnityEngine;

namespace Elarion {
    /// <summary>
    /// Pausable Coroutine
    /// </summary>
    public class ECoroutine {
        private enum CoroutineResult {
            Running,
            Finished,
        }

        public event Action<bool> OnFinished = b => { };

        private readonly MonoBehaviour _owner;
        private readonly IEnumerator _routine;
        private readonly bool _pauseOnInactive;
        private bool _finished;

        private GameObject GameObject {
            get { return _owner.gameObject; }
        }

        private bool Stopped { get; set; }
        public bool Running { get; private set; }
        public bool Paused { get; set; }

        public ECoroutine(IEnumerator routine, MonoBehaviour owner, bool autoStart = true, bool pauseOnInactive = true) {
            _routine = routine;
            _owner = owner;
            _pauseOnInactive = pauseOnInactive;
            _finished = false;
            
            if(autoStart) {
                Start();
            }
        }
        
        public void Start() {
            if(_finished) {
                Debug.LogWarning("This coroutine has already finished. Only paused coroutines can be resumed.", _owner);
                return;
            }
            Running = true;
            _owner.StartCoroutine(CallWrapper());
        }

        public void Stop() {
            Stopped = true;
            Running = false;

            // Synchronously fire the callback and update any dependant state
            OnFinished(Stopped);
        }

        private IEnumerator CallWrapper() {
            while(Running) {
                if(GameObject == null)
                    yield break;
                if(Paused || (_pauseOnInactive && !GameObject.activeInHierarchy))
                    yield return null;
                else {
                    object result;
                    if(ProcessEnumerator(_routine, out result, false) == CoroutineResult.Finished) {
                        Running = false;
                    } else
                        yield return result;
                }
            }

            _finished = true;

            if(!Stopped) {
                // The stop method instantly fires the callback, don't fire it twice
                OnFinished(Stopped);
            }
        }

        private static CoroutineResult ProcessEnumerator(IEnumerator e, out object result, bool finishedInner) {
            if(e == null) {
                result = null;
                return CoroutineResult.Finished;
            }
            
            if((e.Current as IEnumerator) == null) {
                if(finishedInner || e.MoveNext()) {
                    result = e.Current;
                    return CoroutineResult.Running;
                }
                result = null;
                return CoroutineResult.Finished;
            }
            
            var inner = (IEnumerator) e.Current;
            
            if(ProcessEnumerator(inner, out result, false) != CoroutineResult.Finished) {
                return CoroutineResult.Running;
            }
            
            if(e.MoveNext())
                return ProcessEnumerator(e, out result, true);
            
            result = null;
            return CoroutineResult.Finished;
        }
    }
}