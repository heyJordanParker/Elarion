using System;
using UnityEngine;

namespace Elarion.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    public class ConditionalVisibilityAttribute : PropertyAttribute {
        public readonly string visibleConditions;
        public readonly string enableConditions;
        
        public ConditionalVisibilityAttribute(string visibilityConditions = null,
            string enableConditions = null) {

            this.visibleConditions = visibilityConditions;
            this.enableConditions = enableConditions;
        }
        
    }
}