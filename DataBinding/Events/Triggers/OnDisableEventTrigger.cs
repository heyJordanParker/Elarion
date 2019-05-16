namespace Elarion.DataBinding.Events.Triggers {
    public class OnDisableEventTrigger : EventTrigger {
        public void OnEnable() {
            triggeredEvent.Invoke();
        }
    }
}