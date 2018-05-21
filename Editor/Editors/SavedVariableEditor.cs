using System.Reflection;
using Elarion.Attributes;
using Elarion.Saved;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Editors {
    /// <summary>
    /// This fallback editor is used for all SavedVariables. If the application is playing, the editor sends out SavedVariable changed events when the user edits it using the inspector. 
    /// </summary>
    [CustomEditor(typeof(EScriptableObject), true, isFallback = true)]
    public class SavedVariableEditor : UnityEditor.Editor {
        protected bool isSavedVariableEditor;

        private SerializedProperty _valueProperty;

        protected virtual void OnEnable() {
            var type = target.GetType();
            isSavedVariableEditor = target && !type.IsAbstract && !type.IsGenericType &&
                                    type.GetCustomAttributes(typeof(SavedVariableAttribute), true).Length > 0;
            
            _valueProperty = serializedObject.FindProperty("_value");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(!isSavedVariableEditor) {
                return;
            }
            
            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(target, "Changing SavedVariable");
            EditorGUILayout.PropertyField(_valueProperty, new GUIContent("Value"), true);

            if(EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();

                if(!EditorApplication.isPlaying) {
                    return;
                }

                var raiseMethod = target.GetType().GetMethod("Raise", BindingFlags.NonPublic | BindingFlags.Instance);

                var value = target.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                
                raiseMethod.Invoke(target, new []{value.GetValue(target, null)});
            }
        }
    }
}