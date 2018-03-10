using System;
using Elarion.Attributes;
using Elarion.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.UI.PropertyDrawers {

    [CustomPropertyDrawer(typeof(EnumMultipleDropdownAttribute))]
    public class EnumMultipleDropdownDrawer : PropertyDrawer {

        private EnumMultipleDropdownAttribute MultipleDropdownAttribute { get { return ((EnumMultipleDropdownAttribute)attribute); } }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var targetEnum = property.GetBaseProperty<Enum>();

            var name = MultipleDropdownAttribute.name;
            if(!string.IsNullOrEmpty(name)) {
                label.text = name;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            
            var newEnum = EditorGUI.EnumFlagsField(position, label, targetEnum);
            
            if(EditorGUI.EndChangeCheck()) {
                property.intValue = (int) Convert.ChangeType(newEnum, targetEnum.GetType());
            }
            EditorGUI.EndProperty();
        }
    }
}
