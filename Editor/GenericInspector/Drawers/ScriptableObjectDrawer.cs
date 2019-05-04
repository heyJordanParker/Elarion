using System.Collections.Generic;
using Elarion.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion.Editor.GenericInspector.Drawers {
    public class ScriptableObjectDrawer : GenericInspectorDrawer {
        #region Styles
        
        private static GUILayoutOption _uiExpandWidth;
        private static GUIStyle _styleEditBox;
        private static GUIContent _plusIcon;
        private static GUIStyle _plusIconStyle;

        private static GUILayoutOption UIExpandWidth =>
            _uiExpandWidth ?? (_uiExpandWidth = GUILayout.ExpandWidth(true));

        private static GUIStyle StyleEditBox => _styleEditBox ?? 
                                                (_styleEditBox = new GUIStyle("HelpBox")
                                                    {padding = new RectOffset(5, 5, 5, 5)});

        private static GUIContent PlusIcon =>
            _plusIcon ?? (_plusIcon = EditorGUIUtility.IconContent("Toolbar Plus", $"Create a new instance."));

        private static GUIStyle PlusIconStyle => _plusIconStyle ?? (_plusIconStyle = new GUIStyle {
            stretchHeight = false, stretchWidth = false, imagePosition = ImagePosition.ImageOnly,
            padding = new RectOffset(0, 0, 1, 0)
        });
        
        #endregion
        
        private readonly Dictionary<string, GenericInspector> _scriptableObjects =
            new Dictionary<string, GenericInspector>();

        public ScriptableObjectDrawer(GenericInspector inspector, Object target,
            SerializedObject serializedObject) :
            base(inspector, target, serializedObject) {
            var propertyIterator = serializedObject.GetIterator();

            while(propertyIterator.NextVisible(true)) {
                if(propertyIterator.propertyType != SerializedPropertyType.ObjectReference) {
                    continue;
                }

                var propertyType = propertyIterator.GetFieldType();
                if(propertyType == null || !propertyType.IsSubclassOf(typeof(ScriptableObject)))
                    continue;

                UnityEditor.Editor scriptableEditor = null;

                if(propertyIterator.objectReferenceValue != null) {
                    UnityEditor.Editor.CreateCachedEditorWithContext(propertyIterator.objectReferenceValue,
                        serializedObject.targetObject, typeof(GenericInspector),
                        ref scriptableEditor);
                }

                _scriptableObjects.Add(propertyIterator.propertyPath, scriptableEditor as GenericInspector);
            }
        }

        public override bool CanDrawProperty(SerializedProperty property) {
            return _scriptableObjects.ContainsKey(property.propertyPath);
        }

        public override void DrawProperty(SerializedProperty property) {
            var scriptableEditor = _scriptableObjects[property.propertyPath];

            if(scriptableEditor == null && property.objectReferenceValue != null) {
                UnityEditor.Editor editor = null;
                UnityEditor.Editor.CreateCachedEditorWithContext(property.objectReferenceValue,
                    SerializedObject.targetObject, typeof(GenericInspector),
                    ref editor);
                scriptableEditor = editor as GenericInspector;
                _scriptableObjects[property.propertyPath] = scriptableEditor;
            }

            if(scriptableEditor != null && property.objectReferenceValue == null) {
                Object.DestroyImmediate(scriptableEditor);
                _scriptableObjects[property.propertyPath] = null;
                scriptableEditor = null;
            }

            if(scriptableEditor == null) {
                using(new EditorGUILayout.HorizontalScope()) {
                    EditorGUILayout.PropertyField(property, UIExpandWidth);

                    var height = EditorGUI.GetPropertyHeight(property);

                    using(new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false),
                        GUILayout.Width(7f))) {
                        GUILayout.Space(height - EditorGUIUtility.singleLineHeight);
                        var propertyType = property.GetFieldType();
                        if(GUILayout.Button(PlusIcon, PlusIconStyle)) {
                            var createdAsset = Utils.CreateScriptableObject(propertyType, true, false);

                            if(createdAsset == null) {
                                return;
                            }

                            property.objectReferenceInstanceIDValue = createdAsset.GetInstanceID();
                            property.isExpanded = true;
                        }
                    }
                }
            } else {
                EditorGUILayout.PropertyField(property);

                var rectFoldout = GUILayoutUtility.GetLastRect();

                property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, GUIContent.none, true);

                if(!property.isExpanded) {
                    return;
                }

                EditorGUI.indentLevel++;

                using(new EditorGUILayout.VerticalScope(StyleEditBox)) {
                    EditorGUILayout.Space();

                    var indentLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 1;
                    scriptableEditor.serializedObject.Update();
                    scriptableEditor.OnInspectorGUI();
                    scriptableEditor.serializedObject.ApplyModifiedProperties();
                    EditorGUI.indentLevel = indentLevel;

                    EditorGUILayout.Space();
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}