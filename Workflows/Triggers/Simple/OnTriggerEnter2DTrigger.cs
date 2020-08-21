using UnityEngine;

namespace Elarion.Workflows.Events.Triggers.Simple {
    public class OnTriggerEnter2DTrigger : SimpleTrigger {
        private void OnTriggerEnter2D(Collider2D other) {
            FireEvent();
        }
    }
}