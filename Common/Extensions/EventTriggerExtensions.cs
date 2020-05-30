using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Elarion.Extensions {
    public static class EventTriggerExtensions {
        public static void AddEventTrigger(this EventTrigger eventTrigger, UnityAction<BaseEventData> action,
            EventTriggerType eventType) {
            var callback = new EventTrigger.TriggerEvent();
            callback.AddListener(action);

            var entry = new EventTrigger.Entry {callback = callback, eventID = eventType};

            eventTrigger.triggers.Add(entry);
        }
    }
}