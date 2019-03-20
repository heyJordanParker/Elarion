using System;

namespace Elarion.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InspectorButtonAttribute : Attribute {

        public string Title { get; set;  }
        
        public string Label { get; }
        
        public bool AlwaysVisible { get; }

        public InspectorButtonAttribute(string title = null, string label = null, bool alwaysVisible = false) {
            Title = title;
            Label = label;
            AlwaysVisible = alwaysVisible;
        }
    }
}