using Elarion.DataBinding.Events.UnityEvents;
using Elarion.DataBinding.Variables;
using UnityEngine;

namespace Elarion.DataBinding.EventListeners {
    /// <summary>
    /// Fires an event whenever a SavedVector3 changes its' value
    /// </summary>
    public class Vector3EventListener : EventListener<SavedVector3, Vector3UnityEvent, Vector3> { }
}