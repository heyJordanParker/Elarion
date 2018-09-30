using Elarion.Saved.Events.UnityEvents;
using Elarion.Saved.Variables;

namespace Elarion.Saved.Events.Listeners {
    /// <summary>
    /// Fires an event whenever a SavedBool changes its' value
    /// </summary>
    public class BoolEventListener : EventListener<SavedBool, BoolUnityEvent, bool> { }
}