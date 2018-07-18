using System;
using System.Collections.Generic;
using System.Reflection;
using Elarion.Attributes;
using Elarion.Saved;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

namespace Elarion.Editor.Editors {
    // TODO saved list editor (to avoid the type mismatch error)
    
    /// <summary>
    /// This fallback editor is used for all SavedVariables. If the application is playing, the editor sends out SavedVariable changed events when the user edits it using the inspector. 
    /// </summary>
    [CustomEditor(typeof(EScriptableObject), true, isFallback = true)]
    public class SavedVariableEditor : UnityEditor.Editor {
        protected bool isSavedVariableEditor;
        protected bool isSavedListEditor;

        private SerializedProperty _valueProperty;

        protected virtual void OnEnable() {
            var type = target.GetType();
            isSavedVariableEditor = target && !type.IsAbstract && !type.IsGenericType &&
                                    type.GetCustomAttributes(typeof(SavedVariableAttribute), true).Length > 0;
            if(isSavedVariableEditor) {
                _valueProperty = serializedObject.FindProperty("_value");
                return;                
            }
            
            isSavedListEditor = target && !type.IsAbstract && !type.IsGenericType &&
                                    type.GetCustomAttributes(typeof(SavedListAttribute), true).Length > 0;
            
            _valueProperty = serializedObject.FindProperty("values");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(!isSavedVariableEditor && !isSavedListEditor) {
                return;
            }
            
            EditorGUI.BeginChangeCheck();

            Undo.RecordObject(target, "Changing SavedVariable");

            if(isSavedVariableEditor) {
                EditorGUILayout.PropertyField(_valueProperty, new GUIContent("Value"), true);
            } else {
                ObjectReferenceList.Show(_valueProperty);
            }

            if(EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();

                if(!EditorApplication.isPlaying) {
                    return;
                }

                var raiseMethod = target.GetType().GetMethod("Raise", BindingFlags.NonPublic | BindingFlags.Instance);

                PropertyInfo value;
                
                if(isSavedVariableEditor) {
                    value = target.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);                    
                } else {
                    value = target.GetType().GetProperty("values", BindingFlags.NonPublic | BindingFlags.Instance);
                }
                
                raiseMethod.Invoke(target, new []{value.GetValue(target, null)});
            }
        }
    }
}