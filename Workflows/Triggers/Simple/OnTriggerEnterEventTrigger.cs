namespace Elarion.Workflows.Events.Triggers.Simple {
    public class OnEnableTrigger : SimpleTrigger {
        public void OnEnable() {
            FireEvent();
        }
    }
}