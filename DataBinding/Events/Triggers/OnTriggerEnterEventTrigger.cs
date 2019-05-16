namespace Elarion.DataBinding.Events.Triggers {
    public class OnEnableEventTrigger : EventTrigger {
        public void OnEnable() {
            triggeredEvent.Invoke();
        }
    }
}