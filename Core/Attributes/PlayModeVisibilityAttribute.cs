using UnityEngine;

namespace Elarion.Attributes {
    /// <summary>
    /// Draw the selected field in the inspector only in play mode or only outside of play mode.
    /// </summary>
    public class PlayModeVisibilityAttribute : PropertyAttribute {
        public bool DrawInPlayMode { get; }
        public bool LockInPlayMode { get; }
        public bool LockInEditorMode { get; }

        public PlayModeVisibilityAttribute(bool drawInPlayMode = true, bool lockInPlayMode = false, bool lockInEditorMode = false) {
            DrawInPlayMode = drawInPlayMode;
            LockInPlayMode = lockInPlayMode;
            LockInEditorMode = lockInEditorMode;
        }
        
    }
}