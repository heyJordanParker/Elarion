using System;

namespace Elarion.Workflows.Variables.References {
    [Serializable]
    public class BoolReference : SavedValueReference<SavedBool, bool> {
        
        public BoolReference(bool value) : base(value) { }
        
        public static implicit operator bool(BoolReference reference) {
            return reference != null && reference.Value;
        }
    }
}