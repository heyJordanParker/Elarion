using Elarion.DataBinding.Variables.References;
using Elarion.Editor.UI;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    
    [CustomPropertyDrawer(typeof(SavedValueReferenceBase), true)]
    public class SavedValueDrawer : PropertyDrawer {
        
        private readonly string[] _popupOptions =
            {"Constant", "Variable"};

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
            var useConstant = property.FindPropertyRelative("useConstant");
            
            var constantValue = property.FindPropertyRelative("constantValue");
            var variable = property.FindPropertyRelative("variable");
            
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
                
            var result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, _popupOptions, PopupStyle);

            GUI.enabled = true;

            useConstant.boolValue = result == 0;

            EditorGUI.PropertyField(position,
                useConstant.boolValue ? constantValue : variable,
                GUIContent.none);

            if(EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}