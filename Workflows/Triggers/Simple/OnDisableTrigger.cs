namespace Elarion.Workflows.Events.Triggers.Simple {
    public class OnDisableTrigger : SimpleTrigger {
        public void OnEnable() {
            FireEvent();
        }
    }
}