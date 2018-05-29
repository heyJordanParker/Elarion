using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Saved.Events.Listeners {
    /// <summary>
    /// A generic template for building event listeners.
    /// </summary>
    /// <typeparam name="TEvent">The event type. Required for serialization and dynamic binding. Can't be a generic type.</typeparam>
    /// <typeparam name="TUnityEvent">The unity event to show in the inspector. Needs to have the [System.Serializable] attribute. Won't show up in the inspector if set to a generic type. </typeparam>
    /// <typeparam name="TParameter">The parameter that the event will send.</typeparam>
    public abstract class EventListener<TEvent, TUnityEvent, TParameter> :
        MonoBehaviour, IEventListener<TParameter>
        where TEvent : IEventDispatcher<TParameter>
        where TUnityEvent : UnityEvent<TParameter> {
        [SerializeField]
        private TEvent _event;

        [SerializeField]
        private TUnityEvent _onEventRaised;

        private void OnEnable() {
            _event.AddListener(this);
        }

        private void OnDisable() {
            _event.RemoveListener(this);
        }

        public void OnEventRaised(TParameter value) {
            _onEventRaised.Invoke(value);
        }
    }
}