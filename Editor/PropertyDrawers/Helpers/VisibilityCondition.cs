using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elarion.Editor.Extensions;
using UnityEditor;
using UnityEngine;

namespace Elarion.Editor.PropertyDrawers.Helpers {
    public struct VisibilityCondition {
        public readonly string visibilityField;
        public readonly string[] visibilityValues;

        public readonly bool reverseResult;

        public VisibilityCondition(string visibilityControlField, string[] visibilityControlValues,
            bool reverseResult = false) {
            visibilityField = visibilityControlField.Replace(" ", String.Empty);
            visibilityValues = visibilityControlValues;
            this.reverseResult = reverseResult;
        }

        internal static VisibilityCondition[] ParseFromString(string conditionsString) {
            if(string.IsNullOrEmpty(conditionsString)) {
                return new VisibilityCondition[0];
            }
            
            var conditionStrings = conditionsString.Replace(" ", String.Empty).Split(',');
            var visibilityConditionsList = new List<VisibilityCondition>();
            
            try {
                foreach(var conditionString in conditionStrings) {
                    bool reverseResult = false;
                    string separator;
                    string fieldName;
                    string singleValue = null;
                    List<string> fieldValues = new List<string>();

                    if(conditionString.Contains("==")) {
                        separator = "==";
                    } else if(conditionString.Contains("!=")) {
                        separator = "!=";
                        reverseResult = true;
                    }else {
                        separator = null;
                    }

                    if(separator == null) {
                        if(conditionString.Length == 0) {
                            continue;
                        }
                        
                        fieldName = conditionString;
                        singleValue = true.ToString();

                        if(conditionString.Contains("!")) {
                            fieldName = conditionString.Replace("!", "");
                            singleValue = false.ToString();
                        }
                    } else {
                        fieldName = conditionString.Substring(0, conditionString.IndexOf(separator, StringComparison.InvariantCulture));
                        singleValue = conditionString.Substring(conditionString.IndexOf(separator, StringComparison.InvariantCulture) + 2);

                        if(singleValue.Contains("||")) {
                            // Multiple values
                            
                            fieldValues.AddRange(singleValue.Split(new[] { "||" }, StringSplitOptions.None));
                            singleValue = null;
                        }
                    }

                    if(singleValue != null) {
                        fieldValues.Add(singleValue);
                    }

                    visibilityConditionsList.Add(new VisibilityCondition(fieldName, fieldValues.ToArray(), reverseResult));
                }
            } catch(Exception e) {
                Debug.LogWarning("Invalid visibilityConditions provided. Exception " + e);
            }

            return visibilityConditionsList.ToArray();
        }

        internal static bool CheckConditions(VisibilityCondition[] visibilityConditions,
            SerializedProperty property) {
            
            var targetObject = property.GetObject();

            if(targetObject == null) {
                return true;
            }

            var targetObjectType = targetObject.GetType();

            if(visibilityConditions == null) {
                return true;
            }
            
            foreach(var visibilityCondition in visibilityConditions) {
                var visibilityFieldPath =
                    property.propertyPath.Replace(property.name, visibilityCondition.visibilityField);

                var visibilityField = property.serializedObject.FindProperty(visibilityFieldPath);

                var targetField = targetObjectType.GetField(visibilityCondition.visibilityField, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                var targetProperty = targetObjectType.GetProperty(visibilityCondition.visibilityField, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                try {
                    object targetValue;

                    if(visibilityField != null) {
                        // serialized field was found; try getting the value
                        targetValue = targetField.GetValue(targetObject);
                    } else {
                        // no serialized field was found; try a property
                        targetValue = targetProperty.GetValue(targetObject);
                    }

                    var targetValueString = targetValue.ToString();

                    if(targetValue is Enum) {
                        // Enum's strings are just the values; prepend the enum name to get a value that you might see in code
                        targetValueString = targetValue.GetType().Name + '.' + targetValueString;
                    }
                    
                    var visibility = visibilityCondition.visibilityValues.Any(v => v.ToString().Equals(targetValueString, StringComparison.InvariantCultureIgnoreCase));

                    visibility = visibilityCondition.reverseResult ? !visibility : visibility;

                    if(!visibility) {
                        return false;
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