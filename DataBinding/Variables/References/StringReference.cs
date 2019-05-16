using System;

namespace Elarion.DataBinding.Variables.References {
    [Serializable]
    public class StringReference : SavedValueReference<SavedString, string> {
        
        public StringReference(string value) : base(value) { }
        
        public static implicit operator string(StringReference reference) {
            return reference == null ? string.Empty : reference.Value;
        }
    }
}