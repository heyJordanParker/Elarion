using System;

namespace Elarion.Saved.Variables.References {
    [Serializable]
    public class FloatReference : SavedValueReference<SavedFloat, float> {
        
        public FloatReference(float value) : base(value) { }
        
        public static implicit operator float(FloatReference reference) {
            return reference?.Value ?? 0;
        }
    }
}