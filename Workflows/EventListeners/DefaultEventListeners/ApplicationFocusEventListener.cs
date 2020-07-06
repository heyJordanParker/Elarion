using System;
using Elarion.Workflows.Events.UnityEvents;
using UnityEngine;

namespace Elarion.Workflows.EventListeners.DefaultEventListeners {
    public class ApplicationFocusEventListener : MonoBehaviour {
        public BoolUnityEvent onApplicationFocus;

        public event Action<bool> OnApplicationFocusEvent = b => { };

        protected virtual void OnApplicationFocus(bool hasFocus) {
            OnApplicationFocusEvent(hasFocus);
            onApplicationFocus.Invoke(hasFocus);
        }
    }
}