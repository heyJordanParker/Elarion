using Elarion.DataBinding.Events.UnityEvents;
using Elarion.DataBinding.Variables;
using UnityEngine;

namespace Elarion.DataBinding.EventListeners {
    /// <summary>
    /// Fires an event whenever a SavedVector2 changes its' value
    /// </summary>
    public class Vector2EventListener : EventListener<SavedVector2, Vector2UnityEvent, Vector2> { }
}