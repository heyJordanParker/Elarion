using Elarion.UI;
using UnityEditor;
using UnityEngine;
using yaSingleton.Editor;

namespace Elarion.Editor.UI.Editors {
    [CustomEditor(typeof(UIManager), true)]
    public class UIManagerEditor : SingletonEditor {
        private SerializedProperty _selectedObject;

        protected override void OnEnable() {
            base.OnEnable();
            autoDrawSingletonValidation = false;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            EGUI.Readonly(() => {
                var selectedObject = serializedObject.FindProperty("_selectedObject").objectReferenceValue;
                EditorGUILayout.ObjectField("Selected Object", selectedObject, typeof(GameObject), true);
                EditorGUILayout.ObjectField("Focused Component", UIFocusableComponent.FocusedComponent, typeof(UIComponent), true);
            });
            
            DrawSingletonValidation();
        }
        
        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}