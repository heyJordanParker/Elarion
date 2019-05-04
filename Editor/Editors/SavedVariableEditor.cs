using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elarion.Attributes;
using Elarion.Editor.Extensions;
using Elarion.Editor.UI;
using Elarion.Saved;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion.Editor.Editors {
    /// <summary>
    /// This fallback editor is used for all SavedVariables. If the application is playing, the editor sends out SavedVariable changed events when the user edits it using the inspector. 
    /// </summary>
    [CustomEditor(typeof(SavedObject), true, isFallback = true)]
    public class SavedVariableEditor : GenericInspector.GenericInspector {
        protected bool isSavedVariableEditor;
        protected bool isSavedListEditor;

        private SerializedProperty _initialValue;
        private SerializedProperty _runtimeValue;

        protected override void OnEnable() {
            base.OnEnable();

            var type = target.GetType();
            isSavedVariableEditor = target && !type.IsAbstract && !type.IsGenericType &&
                                    type.GetCustomAttributes(typeof(SavedVariableAttribute), true).Length > 0;

            if(isSavedVariableEditor) {
                _initialValue = serializedObject.FindProperty("_initialValue");
                _runtimeValue = serializedObject.FindProperty("_runtimeValue");

                if(_initialValue == null || _runtimeValue == null) {
                    isSavedVariableEditor = false;
                }

                return;
            }

            isSavedListEditor = target && !type.IsAbstract && !type.IsGenericType &&
                                type.GetCustomAttributes(typeof(SavedListAttribute), true).Length > 0;

            _initialValue = serializedObject.FindProperty("_initialValues");
            _runtimeValue = serializedObject.FindProperty("_runtimeValues");

            if(_initialValue == null || _runtimeValue == null) {
                isSavedListEditor = false;
            }
        }

        protected override void DrawProperty(SerializedProperty property) {
            if(isSavedVariableEditor || isSavedListEditor) {
                if(DrawSavedVariable(property)) {
                    return;
                }
            }

            base.DrawProperty(property);
        }

        protected virtual bool DrawSavedVariable(SerializedProperty property) {
            if(property.propertyPath == _initialValue.propertyPath) {
                var guiEnabled = GUI.enabled;
                GUI.enabled = !EditorApplication.isPlaying;

                if(isSavedListEditor && _initialValue.GetFieldType().GetGenericArguments()
                       .First().IsSubclassOf(typeof(Object))) {
                    ObjectReferenceList.Show(_initialValue);
                    
                    GUI.enabled = guiEnabled;
                    return true;
                }

                if(isSavedListEditor && _initialValue.GetFieldType().GetGenericArguments()
                       .First().IsSubclassOf(typeof(Object))) {
                    ObjectReferenceList.Show(_initialValue);
                    GUI.enabled = guiEnabled;
                    return true;
                }

                if(isSavedVariableEditor) {
                    base.DrawProperty(property);
                    GUI.enabled = guiEnabled;
                    return true;
                }

                GUI.enabled = guiEnabled;
            }

            if(property.propertyPath == _runtimeValue.propertyPath) {
                if(!EditorApplication.isPlaying) {
                    return true;
                }
                
                if(isSavedListEditor && _runtimeValue.GetFieldType().GetGenericArguments()
                       .First().IsSubclassOf(typeof(Object))) {
                    ObjectReferenceList.Show(_runtimeValue);
                    return true;
                }

                if(isSavedListEditor && _runtimeValue.GetFieldType().GetGenericArguments()
                       .First().IsSubclassOf(typeof(Object))) {
                    ObjectReferenceList.Show(_runtimeValue);
                    return true;
                }
            }

            return false;
        }

        protected override void OnInspectorChanged() {
            base.OnInspectorChanged();

            if(!EditorApplication.isPlaying) {
                return;
            }
            
            var raiseMethod = target.GetType().GetMethod("Raise",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            if(isSavedVariableEditor) {
                PropertyInfo value = target.GetType().GetProperty("Value",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                
                raiseMethod.Invoke(target, new[] {value.GetValue(target, null)});
                
                return;
            }

            if(isSavedListEditor) {
                raiseMethod.Invoke(target, new[] {target});
            }
        }

        public override bool RequiresConstantRepaint() {
            return true;
        }
    }
}