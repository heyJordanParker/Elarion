using Elarion.DataBinding.Events.UnityEvents;
using Elarion.DataBinding.Variables;

namespace Elarion.DataBinding.EventListeners {
    /// <summary>
    /// Fires an event whenever a SavedBool changes its' value
    /// </summary>
    public class BoolEventListener : EventListener<SavedBool, BoolUnityEvent, bool> { }
}