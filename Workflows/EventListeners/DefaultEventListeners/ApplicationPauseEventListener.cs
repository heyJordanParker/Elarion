using System;
using Elarion.Workflows.Events.UnityEvents;
using UnityEngine;

namespace Elarion.Workflows.EventListeners.DefaultEventListeners {
    public class ApplicationPauseEventListener : MonoBehaviour {
        public BoolUnityEvent onApplicationPause;

        public event Action<bool> OnApplicationPauseEvent = b => { };

        protected virtual void OnApplicationPause(bool pauseStatus) {
            OnApplicationPauseEvent(pauseStatus);
            onApplicationPause.Invoke(pauseStatus);
        }
    }
}