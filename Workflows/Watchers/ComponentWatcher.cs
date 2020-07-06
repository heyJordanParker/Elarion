using Elarion.Attributes;
using UnityEngine;

namespace Elarion.Workflows.Watchers {
    [RequiresConstantRepaint]
    public abstract class ComponentWatcher<TComponent> : ExtendedBehaviour where TComponent : Component {
        [SerializeField, GetComponent]
        protected TComponent component;
    }
}