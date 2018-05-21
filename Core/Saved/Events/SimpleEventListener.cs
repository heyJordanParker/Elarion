using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Saved.Events {
    public class SimpleEventListener : MonoBehaviour {
        [SerializeField]
        private SimpleEvent _event;

        [SerializeField]
        private UnityEvent _onEventRaised;

        private void OnEnable() {
            _event.AddListener(this);
        }

        private void OnDisable() {
            _event.RemoveListener(this);
        }

        public void OnEventRaised() {
            _onEventRaised.Invoke();
        }
    }
}