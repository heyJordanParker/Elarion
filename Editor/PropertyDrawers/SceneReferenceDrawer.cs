using Elarion.Common.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(SceneReferenceAttribute))]
    public class SceneReferenceDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.ObjectField(position, property, label);
        }
    }
}