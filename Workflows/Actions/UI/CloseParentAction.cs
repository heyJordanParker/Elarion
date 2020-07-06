using Elarion.UI;
using UnityEngine;

namespace Elarion.Workflows.Actions.UI {
    [CreateAssetMenu(menuName = "Actions/UI/Close Parent", order = 38)]
    public class CloseParentAction : ScriptableObject {

        /// <summary>
        /// Called from Unity Events
        /// </summary>
        /// <param name="component">The component whose parent should close.</param>
        public void CloseParent(UIComponent component) {
            component.ParentComponent.Close();
        }
    }
}