using Elarion.Attributes;
using Elarion.Editor.PropertyDrawers.Helpers;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(ConditionalLabelAttribute))]
    public class ConditionalLabelDrawer : PropertyDrawer {
        private ConditionalLabelAttribute ConditionalAttribute {
            get { return (ConditionalLabelAttribute) attribute; }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            foreach(var conditionalLabel in ConditionalAttribute.conditionalLabels) {
                var conditions = VisibilityCondition.ParseFromString(conditionalLabel.Value);

                var isVisible = VisibilityCondition.CheckConditions(conditions, property);

                if(!isVisible) {
                    continue;
                }

                label.text = conditionalLabel.Key;
                break;
            }

            EditorGUI.PropertyField(position, property, label);
        }
    }

    [CustomPropertyDrawer(typeof(ConditionalVisibilityAttribute))]
    public class ConditionalVisibilityDrawer : PropertyDrawer {
        private ConditionalVisibilityAttribute ConditionalAttribute {
            get { return (ConditionalVisibilityAttribute) attribute; }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

            var visibilityConditions = VisibilityCondition.ParseFromString(ConditionalAttribute.visibleConditions);

            var isVisible = VisibilityCondition.CheckConditions(visibilityConditions, property);

            // If the target field passes the check or we're just disabling the field - return the actual height
            if(isVisible) {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            // Invisible property; Remove the default spacing
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var visibilityConditions = VisibilityCondition.ParseFromString(ConditionalAttribute.visibleConditions);
            var enabledConditions = VisibilityCondition.ParseFromString(ConditionalAttribute.enableConditions);

            var isVisible = VisibilityCondition.CheckConditions(visibilityConditions, property);
            var isEnabled = VisibilityCondition.CheckConditions(enabledConditions, property);

            var guiEnabled = GUI.enabled;
            GUI.enabled = isEnabled;

            if(isVisible) {
                EditorGUI.PropertyField(position, property, label, true);
            }

            GUI.enabled = guiEnabled;
        }
    }
}