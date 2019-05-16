using UnityEngine;

namespace Elarion.DataBinding {

    /// <inheritdoc />
    /// <summary>
    /// Base class for saved objects. Used for the custom editor.
    /// </summary>
    public class SavedObject : ScriptableObject {
        protected virtual void OnEnable() {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        protected virtual void OnDisable() { }
    }
}