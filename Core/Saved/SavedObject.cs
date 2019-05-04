using UnityEngine;

namespace Elarion.Saved {

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