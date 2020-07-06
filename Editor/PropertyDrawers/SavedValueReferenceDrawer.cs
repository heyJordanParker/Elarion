using System.Reflection;
using Elarion.Workflows.Variables.References;
using Elarion.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    
    [CustomPropertyDrawer(typeof(SavedValueReferenceBase), true)]
    public class SavedValueReferenceDrawer : PropertyDrawer {
        
        private readonly string[] _popupOptions =
            {"Constant Value", "Saved Value"};

        private GUIStyle _popupStyle;

        private GUIStyle PopupStyle {
            get {
                if(_popupStyle == null) {
                    _popupStyle =
                        new GUIStyle(GUI.skin.GetStyle("PaneOptions")) {
                            imagePosition = ImagePosition.ImageOnly
                        };
                }
                
                return _popupStyle;
            }
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Get properties
            var useVariable = property.FindPropertyRelative("useVariable");

            var constantValue = property.FindPropertyRelative("constantValue");
            var variable = property.FindPropertyRelative("variable");

            if(useVariable.boolValue) {
                label.text += $" ({GetSavedValue(property)})";
            }
            
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            // Calculate rect for configuration button
            var buttonRect = new Rect(position);
            buttonRect.yMin += PopupStyle.margin.top;
            buttonRect.width = PopupStyle.fixedWidth + PopupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            
            GUI.enabled = !EditorApplication.isPlayingOrWillChangePlaymode;
                
            var result = EditorGUI.Popup(buttonRect, useVariable.boolValue ? 1 : 0, _popupOptions, PopupStyle);

            GUI.enabled = true;

            useVariable.boolValue = result == 1;

            EditorGUI.PropertyField(position,
                useVariable.boolValue ? variable : constantValue,
                GUIContent.none);

            if(EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        //TODO find a solution that allows combining this with the ScriptableObjectDrawer
        private object GetSavedValue(SerializedProperty property) {
            var obj = property.serializedObject.targetObject;

            var propertyField = obj.GetType().GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.NonPublic);
            var reference = propertyField.GetValue(obj);

            var propertyName = EditorApplication.isPlaying ? "Value" : "InitialValue";

            return reference.GetType().GetProperty(propertyName).GetValue(reference);
        }
    }
}