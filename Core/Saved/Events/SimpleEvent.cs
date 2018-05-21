using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Elarion.Saved.Events {
    
    [CreateAssetMenu(menuName = "Saved Event", order = 31)]
    public class SimpleEvent : EScriptableObject {
        
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
    
    public abstract class RaisableSavedEvent<TEvent, TParameter> : SavedEvent<TParameter> {
        public new void Raise(TParameter value) {
            base.Raise(value);
        }
    }
}