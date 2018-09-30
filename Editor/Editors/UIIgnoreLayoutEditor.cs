using Elarion.UI.Helpers;
using UnityEditor;

namespace Elarion.Editor.Editors {
    [CustomEditor(typeof(UIIgnoreLayout))]
    [CanEditMultipleObjects]
    public class UIIgnoreLayoutEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() { }
    }
}