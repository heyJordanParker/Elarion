using Elarion.Workflows.Events.UnityEvents;
using Elarion.Workflows.Variables;
using UnityEngine;

namespace Elarion.Workflows.EventListeners {
    /// <summary>
    /// Fires an event whenever a SavedVector3 changes its' value
    /// </summary>
    public class Vector3EventListener : EventListener<SavedVector3, Vector3UnityEvent, Vector3> { }
}