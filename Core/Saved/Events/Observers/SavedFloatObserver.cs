using Elarion.Saved.Events.Listeners;
using Elarion.Saved.Events.UnityEvents;
using Elarion.Saved.Variables;

namespace Elarion.Saved.Events.Observers {
    /// <summary>
    /// Fires an event whenever a SavedFloat changes its' value
    /// </summary>
    public class SavedFloatObserver : EventListener<SavedFloat, FloatUnityEvent, float> { }
}