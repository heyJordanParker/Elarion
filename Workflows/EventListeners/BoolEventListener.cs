using Elarion.Workflows.Events.UnityEvents;
using Elarion.Workflows.Variables;

namespace Elarion.Workflows.EventListeners {
    /// <summary>
    /// Fires an event whenever a SavedBool changes its' value
    /// </summary>
    public class BoolEventListener : EventListener<SavedBool, BoolUnityEvent, bool> { }
}