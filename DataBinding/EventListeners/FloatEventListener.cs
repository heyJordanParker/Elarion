using Elarion.DataBinding.Events.UnityEvents;
using Elarion.DataBinding.Variables;

namespace Elarion.DataBinding.EventListeners {
    /// <summary>
    /// Fires an event whenever a SavedFloat changes its' value
    /// </summary>
    public class FloatEventListener : EventListener<SavedFloat, FloatUnityEvent, float> { }
}