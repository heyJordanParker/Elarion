using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elarion.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    public class ConditionalVisibilityAttribute : PropertyAttribute {
        public readonly VisibilityCondition[] visibilityConditions;
        public readonly bool needsAllConditions;
        public readonly bool hide;

        /// <summary>
        /// Conditional visibility in the Inspector. You can specify one or many visibility conditions that will determine whether or not this field will be rendered in the inspector. Note: doesn't support arrays and doesn't work with other attributes.
        /// </summary>
        /// <param name="needsAllConditions">Only shows the field if all the conditions are satisfied. If set to false, shows the field if just one condition is satisfied.</param>
        /// <param name="visibilityConditionsString">String containing multiple conditions. Format is as follows: "fieldName1, fieldName2=fieldValue2, field3!=fieldValue3". Format supports basic true checks - fieldName1, explicit value checks (equals and not equals) - field2, field3.</param>
        /// <param name="hide">Should this field be hidden if the field's value differs. If set to false, this field gets disabled instead.</param>
        public ConditionalVisibilityAttribute(string visibilityConditionsString, bool needsAllConditions = true,
            bool hide = true) {
            var conditionStrings = visibilityConditionsString.Replace(" ", String.Empty).Split(',');
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
                Debug.LogWarning("Invalid visibilityConditionsString provided. Exception " + e);
            }


            visibilityConditions = visibilityConditionsList.ToArray();
            this.hide = hide;
            this.needsAllConditions = needsAllConditions;
        }
    }

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
        
        
    }
}