using System;

namespace Elarion.Attributes {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class InspectorButtonValidatorAttribute : Attribute {

        public string Title { get; set; }

        public InspectorButtonValidatorAttribute(string title = null) {
            Title = title;
        }

    }
}