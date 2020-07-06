using System;
using UnityEngine;

namespace Elarion.Workflows.Variables.References {
    [Serializable]
    public class Vector2Reference : SavedValueReference<SavedVector2, Vector2> {
        
        public Vector2Reference(Vector2 value) : base(value) { }
        
        public static implicit operator Vector2(Vector2Reference reference) {
            return reference == null ? Vector2.zero : reference.Value;
        }
        
        public static implicit operator Vector3(Vector2Reference reference) {
            return reference == null ? Vector2.zero : reference.Value;
        }
    }
}