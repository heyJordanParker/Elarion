using System;

namespace Elarion.DataBinding.Events {
    public abstract class SavedEvent<TParameter> : SavedObject {
        
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