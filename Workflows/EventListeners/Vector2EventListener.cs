using Elarion.Workflows.Events.UnityEvents;
using Elarion.Workflows.Variables;
using UnityEngine;

namespace Elarion.Workflows.EventListeners {
    /// <summary>
    /// Fires an event whenever a SavedVector2 changes its' value
    /// </summary>
    public class Vector2EventListener : EventListener<SavedVector2, Vector2UnityEvent, Vector2> { }
}