using Elarion.Saved.Events.UnityEvents;
using Elarion.Saved.Variables;

namespace Elarion.Saved.Events.Listeners {
    /// <summary>
    /// Fires an event whenever a SavedFloat changes its' value
    /// </summary>
    public class FloatEventListener : EventListener<SavedFloat, FloatUnityEvent, float> { }
}