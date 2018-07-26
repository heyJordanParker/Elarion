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

            _selectedObject = serializedObject.FindProperty("_selectedObject");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            EGUI.Readonly(() => {
                EditorGUILayout.ObjectField("Selected Object", _selectedObject.objectReferenceValue, typeof(GameObject), true);
            });
            
            DrawSingletonValidation();
        }
        
        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}