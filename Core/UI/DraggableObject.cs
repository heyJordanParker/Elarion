using Elarion;
using Elarion.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elarion.UI {
    [RequireComponent(typeof(EventTrigger))]
    public class DraggableObject : BasicUIElement {
        private EventTrigger _eventTrigger;

        protected override void Awake() {
            base.Awake();
            _eventTrigger = GetComponent<EventTrigger>();
            _eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
        }

        void OnDrag(BaseEventData data) {
            var ped = (PointerEventData)data;
            Transform.Translate(ped.delta);
        }
    }
}