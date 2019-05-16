using UnityEngine;

namespace Elarion.DataBinding.Events.Triggers {
    public class OnTriggerEnter2DEventTrigger : EventTrigger {
        private void OnTriggerEnter2D(Collider2D other) {
            triggeredEvent.Invoke();
        }
    }
}