using UnityEngine;

namespace Elarion.DataBinding.Events.Triggers {
    public class OnTriggerExit2DEventTrigger : EventTrigger {
        private void OnTriggerExit2D(Collider2D other) {
            triggeredEvent.Invoke();
        }
    }
}