using System.Collections.Generic;
using UnityEngine;

namespace Elarion.Attributes {
    public class ConditionalLabelAttribute : PropertyAttribute {

        public readonly Dictionary<string, string> conditionalLables;
        
        public ConditionalLabelAttribute(string label, string visibilityConditions, params string[] labelConditionPairs) {
            
            conditionalLables = new Dictionary<string, string>();
            
            if(labelConditionPairs.Length % 2 != 0) {
                Debug.LogWarning("Label count doesn't match condition count. Unable to create ConditionalLabelAttribute.");
                return;
            }

            if(!string.IsNullOrEmpty(label) && !string.IsNullOrEmpty(visibilityConditions)) {
                conditionalLables.Add(label, visibilityConditions);
            }

            for(int i = 0; i < labelConditionPairs.Length; i += 2) {
                label = labelConditionPairs[i];
                visibilityConditions = labelConditionPairs[i + 1];
                
                conditionalLables.Add(label, visibilityConditions);
            }
        }
    }
}