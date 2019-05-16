using System;
using Elarion.Common.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(StringDropdownAttribute))]
    public class StringDropdownDrawer : PropertyDrawer {

        private StringDropdownAttribute EnumAttribute { get { return ((StringDropdownAttribute)attribute); } }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return base.GetPropertyHeight(property, label); }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var propertyName = EnumAttribute.name;
            if(string.IsNullOrEmpty(propertyName))
                propertyName = property.name;

            int currentIndex = Array.IndexOf(EnumerationNames, property.stringValue);

            if(currentIndex < 0) {
                property.stringValue = EnumerationNames[0];
                currentIndex = 0;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            int enumValueIndex = EditorGUI.Popup(position, propertyName, currentIndex, EnumerationNames);
            if(EditorGUI.EndChangeCheck()) property.stringValue = EnumerationNames[enumValueIndex];
            EditorGUI.EndProperty();
        }

        public string[] EnumerationNames {
            get { return EnumAttribute.choices; }
        }

    }
}