using System;
using UnityEngine;

namespace Elarion.DataBinding.Variables.References {
    [Serializable]
    public class Vector3Reference : SavedValueReference<SavedVector3, Vector3> {
        
        public Vector3Reference(Vector3 value) : base(value) { }
        
        public static implicit operator Vector3(Vector3Reference reference) {
            return reference == null ? Vector3.zero : reference.Value;
        }
        
        public static implicit operator Vector2(Vector3Reference reference) {
            return reference == null ? Vector3.zero : reference.Value;
        }
    }
}