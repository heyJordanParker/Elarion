using System.Text.RegularExpressions;
using Elarion.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(RegexAttribute))]
    public class RegexDrawer : PropertyDrawer {
        // These constants describe the height of the help box and the text field.
        private const int HelpHeight = 30;
        private const int TextHeight = 16;

        // Provide easy access to the RegexAttribute for reading information from it.
        private RegexAttribute RegexAttribute {
            get { return (RegexAttribute) attribute; }
        }

        // Here you must define the height of your property drawer. Called by Unity.
        public override float GetPropertyHeight(SerializedProperty prop,
            GUIContent label) {
            if(IsValid(prop))
                return base.GetPropertyHeight(prop, label);
            return base.GetPropertyHeight(prop, label) + HelpHeight;
        }

        // Here you can define the GUI for your property drawer. Called by Unity.
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
            // Adjust height of the text field
            var textFieldPosition = position;
            textFieldPosition.height = TextHeight;
            DrawTextField(textFieldPosition, prop, label);

            // Adjust the help box position to appear indented underneath the text field.
            var helpPosition = EditorGUI.IndentedRect(position);
            helpPosition.y += TextHeight;
            helpPosition.height = HelpHeight;
            DrawHelpBox(helpPosition, prop);
        }

        private void DrawTextField(Rect position, SerializedProperty prop, GUIContent label) {
            // Draw the text field control GUI.
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.TextField(position, label, prop.stringValue);
            if(EditorGUI.EndChangeCheck())
                prop.stringValue = value;
        }

        private void DrawHelpBox(Rect position, SerializedProperty prop) {
            // No need for a help box if the pattern is valid.
            if(IsValid(prop))
                return;

            EditorGUI.HelpBox(position, RegexAttribute.helpMessage, MessageType.Error);
        }

        // Test if the propertys string value matches the regex pattern.
        private bool IsValid(SerializedProperty prop) {
            return Regex.IsMatch(prop.stringValue, RegexAttribute.pattern);
        }
    }
}