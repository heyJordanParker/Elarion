using Elarion.Workflows.Events.UnityEvents;
using Elarion.Workflows.Variables;

namespace Elarion.Workflows.Triggers {
    public class BoolTrigger : Trigger<SavedBool, BoolUnityEvent, bool> { }
}