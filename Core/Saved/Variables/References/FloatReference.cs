using System;

namespace Elarion.Saved.Variables.References {
    [Serializable]
    public class FloatReference : SavedValueReference<SavedFloat, float> {
        
        public FloatReference(float value) : base(value) { }
        
        public static implicit operator float(FloatReference reference) {
            return reference == null ? 0 : reference.Value;
        }
    }
    
    [Serializable]
    public class BoolReference : SavedValueReference<SavedBool, bool> {
        
        public BoolReference(bool value) : base(value) { }
        
        public static implicit operator bool(BoolReference reference) {
            return reference != null && reference.Value;
        }
    }
}