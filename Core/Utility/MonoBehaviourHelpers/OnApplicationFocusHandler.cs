using System;
using Elarion.Saved.Events.UnityEvents;
using UnityEngine;

namespace Elarion.Utility.MonoBehaviourHelpers {
    public class OnApplicationFocusHandler : MonoBehaviour {
        public BoolUnityEvent onApplicationFocus;

        public event Action<bool> OnApplicationFocusEvent = b => { };

        protected virtual void OnApplicationFocus(bool hasFocus) {
            OnApplicationFocusEvent(hasFocus);
            onApplicationFocus.Invoke(hasFocus);
        }
    }
}