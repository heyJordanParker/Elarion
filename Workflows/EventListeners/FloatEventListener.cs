using Elarion.Workflows.Events.UnityEvents;
using Elarion.Workflows.Variables;

namespace Elarion.Workflows.EventListeners {
    /// <summary>
    /// Fires an event whenever a SavedFloat changes its' value
    /// </summary>
    public class FloatEventListener : EventListener<SavedFloat, FloatUnityEvent, float> { }
}