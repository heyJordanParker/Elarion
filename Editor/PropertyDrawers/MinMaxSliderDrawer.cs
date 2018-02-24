using Elarion.Attributes;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer {
        
        private static readonly GUIStyle LabelStyle = new GUIStyle("label");
        
        private MinMaxSliderAttribute Attribute {
            get { return (MinMaxSliderAttribute) attribute; }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + 2;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            if(property.propertyType != SerializedPropertyType.Vector2) {
                EditorGUI.LabelField(rect, label,
                    "Invalid property. Use the MinMaxSlider only with Vector2 fields.");
                return;
            }
            
            rect.height = EditorGUIUtility.singleLineHeight;

            var min = property.vector2Value.x;
            var max = property.vector2Value.y;

            EditorGUI.BeginChangeCheck();

            EditorGUI.MinMaxSlider(rect, label, ref min, ref max, Attribute.minValue, Attribute.maxValue);

            rect.y += EditorGUIUtility.singleLineHeight + 2;
            
            rect.x += EditorGUIUtility.labelWidth;
            rect.width -= EditorGUIUtility.labelWidth;
            
            rect.width = rect.width / 2 - 5;

            var inLabel = new GUIContent("Min");
            var inRect = rect;

            inRect.width = LabelStyle.CalcSize(inLabel).x + 5;
            
            EditorGUI.LabelField(inRect, inLabel);

            inRect.x += inRect.width; 
            inRect.width = rect.width - inRect.width;

            if(Attribute.minValueString == null || Mathf.RoundToInt(Attribute.minValue - min) != 0) {
                min = EditorGUI.IntField(inRect, GUIContent.none, (int) min);
            } else {
                EditorGUI.LabelField(inRect, GUIContent.none, new GUIContent(Attribute.minValueString));
            }

            rect.x += rect.width + 10;

            inRect = rect;
            inLabel.text = "Max";
            
            inRect.width = LabelStyle.CalcSize(inLabel).x + 5;
            
            EditorGUI.LabelField(inRect, inLabel);

            inRect.x += inRect.width; 
            inRect.width = rect.width - inRect.width;

            if(Attribute.maxValueString == null || Mathf.RoundToInt(Attribute.maxValue - max) != 0) {
                max = EditorGUI.IntField(inRect, GUIContent.none, (int) max);
            } else {
                EditorGUI.LabelField(inRect, GUIContent.none, new GUIContent(Attribute.maxValueString));
            }

            if(EditorGUI.EndChangeCheck()) {
                property.vector2Value = new Vector2(min, max);
            }
        }
    }
}