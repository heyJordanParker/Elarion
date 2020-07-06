using System;
using System.Collections.Generic;
using Elarion.Workflows.EventListeners;
using UnityEngine;

namespace Elarion.Workflows.Events {
     
    [CreateAssetMenu(menuName = "Events/Simple Event", order = 35)]
    public class SimpleEvent : SavedObject {
        
        private readonly List<SimpleEventListener> _eventListeners =
            new List<SimpleEventListener>();

        private event Action ChangedEvent = () => { };

        public virtual void Raise() {
            ChangedEvent();
            
            for(int i = _eventListeners.Count - 1; i >= 0; i--) {
                _eventListeners[i].OnEventRaised();
            }
        }

        public void Subscribe(Action onEventRaised) {
            ChangedEvent += onEventRaised;
        }

        public void Unsubscribe(Action onEventRaised) {
            ChangedEvent -= onEventRaised;
        }

        public void AddListener(SimpleEventListener listener) {
            if(!_eventListeners.Contains(listener)) {
                _eventListeners.Add(listener);
            }
        }

        public void RemoveListener(SimpleEventListener listener) {
            if(_eventListeners.Contains(listener)) {
                _eventListeners.Remove(listener);
            }
        }
    }
}