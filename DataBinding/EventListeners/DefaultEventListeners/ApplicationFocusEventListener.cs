using System;
using Elarion.DataBinding.Events.UnityEvents;
using UnityEngine;

namespace Elarion.DataBinding.EventListeners.DefaultEventListeners {
    public class ApplicationFocusEventListener : MonoBehaviour {
        public BoolUnityEvent onApplicationFocus;

        public event Action<bool> OnApplicationFocusEvent = b => { };

        protected virtual void OnApplicationFocus(bool hasFocus) {
            OnApplicationFocusEvent(hasFocus);
            onApplicationFocus.Invoke(hasFocus);
        }
    }
}