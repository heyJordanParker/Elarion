namespace Elarion.Workflows.Events.Triggers {
    public class OnEnableEventTrigger : EventTrigger {
        public void OnEnable() {
            triggeredEvent.Invoke();
        }
    }
}