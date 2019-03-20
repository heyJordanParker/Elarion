// Simple helper class that allows you to serialize System.Type objects.
// Use it however you like, but crediting or even just contacting the author would be appreciated (Always 
// nice to see people using your stuff!)
//
// Written by Bryan Keiren (http://www.bryankeiren.com)

using System;
using UnityEngine;

namespace Elarion.Utility {
    [Serializable]
    public class SerializableType {
        [SerializeField]
        private string _assemblyQualifiedName;

        private Type _type;

        public Type Type {
            get {
                if(_type == null) {
                    _type = Type.GetType(_assemblyQualifiedName);
                }

                return _type;
            }
        }

        public SerializableType(Type systemType) {
            _type = systemType;
            _assemblyQualifiedName = systemType.AssemblyQualifiedName;
        }

        public override bool Equals(System.Object obj) {
            SerializableType temp = obj as SerializableType;
            return (object) temp != null && Equals(temp);
        }

        public override int GetHashCode() {
            return Type != null ? Type.GetHashCode() ^ 367 : 0;
        }

        public bool Equals(SerializableType other) {
            return other.Type == Type && other._assemblyQualifiedName == _assemblyQualifiedName;
        }

        public static bool operator ==(SerializableType a, SerializableType b) {
            // If both are null, or both are same instance, return true.
            if(ReferenceEquals(a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if(((object) a == null) || ((object) b == null)) {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(SerializableType a, SerializableType b) {
            return !(a == b);
        }

        public static implicit operator Type(SerializableType serializableType) {
            return serializableType?.Type;
        }
    }
}