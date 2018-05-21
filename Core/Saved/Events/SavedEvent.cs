using System;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion.Saved.Events {
    public abstract class SavedEvent<TParameter> : EScriptableObject, IEventDispatcher<TParameter> {
        
        private readonly List<IEventListener<TParameter>> _eventListeners =
            new List<IEventListener<TParameter>>();

        private event Action<TParameter> Event = v => { };

        protected void Raise(TParameter value) {
            Event(value);
            
            for(int i = _eventListeners.Count - 1; i >= 0; i--) {
                _eventListeners[i].OnEventRaised(value);
            }
        }
        
        public virtual void Subscribe(Action<TParameter> onEventRaised) {
            Event += onEventRaised;
        }

        public virtual void Unsubscribe(Action<TParameter> onEventRaised) {
            Event -= onEventRaised;
        }

        public virtual void AddListener(IEventListener<TParameter> listener) {
            if(!_eventListeners.Contains(listener)) {
                _eventListeners.Add(listener);
            }
        }

        public virtual void RemoveListener(IEventListener<TParameter> listener) {
            if(_eventListeners.Contains(listener)) {
                _eventListeners.Remove(listener);
            }
        }
    }
}