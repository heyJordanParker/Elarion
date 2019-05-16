using Elarion.DataBinding.Events.UnityEvents;
using Elarion.DataBinding.Variables;

namespace Elarion.DataBinding.EventListeners {
    /// <summary>
    /// Fires an event whenever a SavedInt changes its' value
    /// </summary>
    public class IntEventListener : EventListener<SavedInt, IntUnityEvent, int> { }
}