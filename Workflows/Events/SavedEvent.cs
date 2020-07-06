using System;

namespace Elarion.Workflows.Events {
    public abstract class SavedEvent<TParameter> : SimpleEvent {
        
        private event Action<TParameter> Event = v => { };

        public void Raise(TParameter value) {
            base.Raise();
            Event(value);
        }

        public override void Raise() {
            Raise(default);
        }

        public virtual void Subscribe(Action<TParameter> onEventRaised) {
            Event += onEventRaised;
        }

        public virtual void Unsubscribe(Action<TParameter> onEventRaised) {
            Event -= onEventRaised;
        }
    }
}