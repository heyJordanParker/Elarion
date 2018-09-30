using Elarion.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(GetComponentAttribute))]
    public class GetComponentDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            // Invisible field; Remove the default spacing
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) { }
    }
}