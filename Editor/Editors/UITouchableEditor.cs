using Elarion.UI.Utils;
using UnityEditor;

namespace Elarion.Editor.Editors {
    [CustomEditor(typeof(Touchable))]
    [CanEditMultipleObjects]
    public class UITouchableEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() { }
    }
}
