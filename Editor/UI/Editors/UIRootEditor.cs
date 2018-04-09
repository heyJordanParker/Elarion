using Elarion.UI;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.UI.Editors {
    [CustomEditor(typeof(UIManager), true)]
    public class UIRootEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Focused Component", UIFocusableComponent.FocusedComponent, typeof(UIComponent), true);
            GUI.enabled = true;
        }
        
        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}