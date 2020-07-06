using Elarion.Workflows.Events.UnityEvents;
using Elarion.Workflows.Variables;

namespace Elarion.Workflows.EventListeners {
    /// <summary>
    /// Fires an event whenever a SavedInt changes its' value
    /// </summary>
    public class IntEventListener : EventListener<SavedInt, IntUnityEvent, int> { }
}