using System;
using Elarion.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.UI.PropertyDrawers {
    [CustomPropertyDrawer(typeof(EnumDropdownAttribute))]
    public class EnumDropdownDrawer : PropertyDrawer {
        private EnumDropdownAttribute EnumAttribute {
            get { return ((EnumDropdownAttribute) attribute); }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            var propertyName = EnumAttribute.name;
            if(string.IsNullOrEmpty(propertyName))
                propertyName = property.name;

            var type = EnumAttribute.type;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            int enumValueIndex = EditorGUI.Popup(position, propertyName, property.intValue, GetEnumerationNames(type));

            if(EditorGUI.EndChangeCheck()) {
                property.intValue = enumValueIndex;
            }

            EditorGUI.EndProperty();
        }

        public string[] GetEnumerationNames(Type type) {
            return Enum.GetNames(type);
        }
    }
}