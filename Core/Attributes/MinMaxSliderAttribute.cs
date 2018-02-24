using UnityEngine;

namespace Elarion.Attributes {
    public class MinMaxSliderAttribute : PropertyAttribute {
        public readonly float minValue;
        public readonly float maxValue;
        public readonly string maxValueString;
        public readonly string minValueString;

        public MinMaxSliderAttribute(float minValue, float maxValue, string minValueString = null, string maxValueString = null) {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.minValueString = minValueString;
            this.maxValueString = maxValueString;
        }
    }
}