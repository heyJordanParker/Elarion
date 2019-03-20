using Elarion.Saved.Events.UnityEvents;
using Elarion.Saved.Variables;
using UnityEngine;

namespace Elarion.Saved.Events.Listeners {
    /// <summary>
    /// Fires an event whenever a SavedVector3 changes its' value
    /// </summary>
    public class Vector3EventListener : EventListener<SavedVector3, Vector3UnityEvent, Vector3> { }
}