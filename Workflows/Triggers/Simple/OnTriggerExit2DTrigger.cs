using UnityEngine;

namespace Elarion.Workflows.Events.Triggers.Simple {
    public class OnTriggerExit2DTrigger : SimpleTrigger {
        private void OnTriggerExit2D(Collider2D other) {
            FireEvent();
        }
    }
}