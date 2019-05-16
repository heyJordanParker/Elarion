using System;
using System.Collections.Generic;
using Elarion.DataBinding.EventListeners;
using UnityEngine;

namespace Elarion.DataBinding.Events {
     
    [CreateAssetMenu(menuName = "Saved/Events/Simple Event", order = -11)]
    public class SimpleEvent : SavedObject {
        
        private readonly List<SimpleEventListener> _eventListeners =
            new List<SimpleEventListener>();

        private event Action Event = () => { };

        public void Raise() {
            Event();
            
            for(int i = _eventListeners.Count - 1; i >= 0; i--) {
                _eventListeners[i].OnEventRaised();
            }
        }

        public void Subscribe(Action onEventRaised) {
            Event += onEventRaised;
        }

        public void Unsubscribe(Action onEventRaised) {
            Event -= onEventRaised;
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