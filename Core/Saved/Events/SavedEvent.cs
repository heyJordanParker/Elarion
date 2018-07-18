using System;
using System.Collections.Generic;
using Elarion.Saved.Events.Listeners;

namespace Elarion.Saved.Events {
    public abstract class SavedEvent<TParameter> : EScriptableObject {
        
        private event Action<TParameter> Event = v => { };

        public void Raise(TParameter value) {
            Event(value);
        }
        
        public virtual void Subscribe(Action<TParameter> onEventRaised) {
            Event += onEventRaised;
        }

        public virtual void Unsubscribe(Action<TParameter> onEventRaised) {
            Event -= onEventRaised;
        }
    }
}