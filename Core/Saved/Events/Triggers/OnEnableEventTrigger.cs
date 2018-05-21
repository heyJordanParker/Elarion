namespace Elarion.Saved.Events.Triggers {
    public class OnEnableEventTrigger : EventTrigger {
        public void OnEnable() {
            triggeredEvent.Invoke();
        }
    }
}