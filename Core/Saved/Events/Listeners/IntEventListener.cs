using Elarion.Saved.Events.UnityEvents;
using Elarion.Saved.Variables;

namespace Elarion.Saved.Events.Listeners {
    /// <summary>
    /// Fires an event whenever a SavedInt changes its' value
    /// </summary>
    public class IntEventListener : EventListener<SavedInt, IntUnityEvent, int> { }
}