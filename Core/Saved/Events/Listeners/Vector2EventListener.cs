using Elarion.Saved.Events.UnityEvents;
using Elarion.Saved.Variables;
using UnityEngine;

namespace Elarion.Saved.Events.Listeners {
    /// <summary>
    /// Fires an event whenever a SavedVector2 changes its' value
    /// </summary>
    public class Vector2EventListener : EventListener<SavedVector2, Vector2UnityEvent, Vector2> { }
}