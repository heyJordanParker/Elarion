using System;

namespace Elarion.DataBinding.Variables.References {
    [Serializable]
    public class IntReference : SavedValueReference<SavedInt, int> {
        
        public IntReference(int value) : base(value) { }
        
        public static implicit operator int(IntReference reference) {
            return reference?.Value ?? 0;
        }
    }
}