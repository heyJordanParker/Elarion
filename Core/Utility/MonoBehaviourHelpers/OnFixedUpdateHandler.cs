using System;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Utility.MonoBehaviourHelpers {
    public class OnFixedUpdateHandler : MonoBehaviour {
        public UnityEvent onFixedUpdate;

        public event Action OnFixedUpdateEvent = () => { };

        protected virtual void FixedUpdate() {
            OnFixedUpdateEvent();
            onFixedUpdate.Invoke();
        }
    }
}