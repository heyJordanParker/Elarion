using Elarion.Colors;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.Editors {
    [CustomPropertyDrawer(typeof(ColorHSL))]
    public class ColorHSLEditor : PropertyDrawer {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            EditorGUI.BeginProperty(position, label, property);
            
            var h = property.FindPropertyRelative("_h");
            var s = property.FindPropertyRelative("_s");
            var l = property.FindPropertyRelative("_l");
            var a = property.FindPropertyRelative("_a");

            EditorGUI.BeginChangeCheck();
            
            var newColor = EditorGUI.ColorField(position, label, new ColorHSL(h.floatValue, s.floatValue, l.floatValue, a.floatValue).ToColor(), true,
                true, false);

            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(property.serializedObject.targetObject, "Changing color");
                var newColorHSL = new ColorHSL(newColor);
                h.floatValue = newColorHSL.H;
                s.floatValue = newColorHSL.S;
                l.floatValue = newColorHSL.L;
                a.floatValue = newColorHSL.A;

                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.EndProperty();
        }
    }
}