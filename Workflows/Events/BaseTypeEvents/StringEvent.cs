using UnityEngine;

namespace Elarion.Workflows.Events.BaseTypeEvents {
    [CreateAssetMenu(menuName = "Events/String Event", order = 35)]
    public class StringEvent : SavedEvent<string> { }
}