using System;
using Elarion.Saved.Events.UnityEvents;
using UnityEngine;

namespace Elarion.Utility.MonoBehaviourHelpers {
    public class OnApplicationPauseHandler : MonoBehaviour {
        public BoolUnityEvent onApplicationPause;

        public event Action<bool> OnApplicationPauseEvent = b => { };

        protected virtual void OnApplicationPause(bool pauseStatus) {
            OnApplicationPauseEvent(pauseStatus);
            onApplicationPause.Invoke(pauseStatus);
        }
    }
}