using System;
using System.Linq;
using System.Reflection;
using Elarion.Attributes;
using Elarion.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(ConditionalVisibilityAttribute))]
    public class ConditionalVisibilityDrawer : PropertyDrawer {
        private ConditionalVisibilityAttribute ConditionalAttribute {
            get { return (ConditionalVisibilityAttribute) attribute; }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var isEnabled = IsEnabled(ConditionalAttribute, property);

            // If the target field passes the check or we're just disabling the field - return the actual height
            if(!ConditionalAttribute.hide || isEnabled) {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            // Invisible property; Remove the default spacing
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var isEnabled = IsEnabled(ConditionalAttribute, property);

            var guiEnabled = GUI.enabled;
            GUI.enabled = isEnabled;

            if(!ConditionalAttribute.hide || isEnabled) {
                EditorGUI.PropertyField(position, property, label, true);
            }

            GUI.enabled = guiEnabled;
        }

        private bool IsEnabled(ConditionalVisibilityAttribute cAttribute,
            SerializedProperty property) {
            
            var targetObject = property.GetTargetObject();

            if(targetObject == null) {
                return true;
            }

            var targetObjectType = targetObject.GetType();
            
            foreach(var visibilityCondition in cAttribute.visibilityConditions) {
                var visibilityFieldPath =
                    property.propertyPath.Replace(property.name, visibilityCondition.visibilityField);

                var visibilityField = property.serializedObject.FindProperty(visibilityFieldPath);

                if(visibilityField == null) {
                    Debug.LogWarning(
                        "VisibilityCondition: failed getting a field the " +
                        visibilityCondition.visibilityField + " field. Please provide a valid name.");
                    return false;
                }

                var targetField = targetObjectType.GetField(visibilityCondition.visibilityField, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                try {
                    var targetValue = targetField.GetValue(targetObject);

                    var targetValueString = targetValue.ToString();
                    
                    var visibility = visibilityCondition.visibilityValues.Any(v => v.ToString().Equals(targetValueString, StringComparison.InvariantCultureIgnoreCase));
                    
                    if(targetValue is Enum) {
                        // Enums might or might not be prefixed with the enum name; we only care about the value
                        visibility = visibilityCondition.visibilityValues.Any(v => v.ToString().EndsWith(targetValueString));
                    }

                    visibility = visibilityCondition.reverseResult ? !visibility : visibility;

                    if(cAttribute.needsAllConditions && !visibility) {
                        return false;
                    }

                    if(!cAttribute.needsAllConditions && visibility) {
                        return true;
                    }
                } catch(Exception e) {
                    Debug.LogWarning("VisibilityCondition: failed getting the value of the " +
                                     visibilityCondition.visibilityField +
                                     " field. Exception: " + e);

                    return false;
                }
            }

            return true;
        }
    }
}