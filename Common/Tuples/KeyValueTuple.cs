using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion.Common.Tuples {
    /// <summary>
    /// Serializable KeyValue Tuple
    /// </summary>
    /// <typeparam name="T1">Key Type</typeparam>
    /// <typeparam name="T2">Value Type</typeparam>
    [Serializable]
    public class KeyValueTuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable,
        IEquatable<KeyValueTuple<T1, T2>> where T1 : IComparable where T2 : IComparable {
        [SerializeField]
        private T1 _key;

        [SerializeField]
        private T2 _value;

        public T1 Key {
            get => _key;
            set => _key = value;
        }

        public T2 Value {
            get => _value;
            set => _value = value;
        }

        public bool Equals(object other, IEqualityComparer comparer) {
            return ((IStructuralEquatable) this).Equals(other, EqualityComparer<object>.Default);
        }

        public int GetHashCode(IEqualityComparer comparer) {
            unchecked {
                return (EqualityComparer<T1>.Default.GetHashCode(_key) * 397) ^
                       EqualityComparer<T2>.Default.GetHashCode(_value);
            }
        }

        public int CompareTo(object other, IComparer comparer) {
            if(!(other is KeyValueTuple<T1, T2> thatTuple)) {
                return 1;
            }

            var c = 0;
            c = comparer.Compare(_key, thatTuple.Key);
            if(c != 0) {
                return c;
            }

            return comparer.Compare(_value, thatTuple.Value);
        }

        public bool Equals(KeyValueTuple<T1, T2> other) {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return EqualityComparer<T1>.Default.Equals(_key, other._key) &&
                   EqualityComparer<T2>.Default.Equals(_value, other._value);
        }

        public override bool Equals(object obj) {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((KeyValueTuple<T1, T2>) obj);
        }

        public override int GetHashCode() {
            return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<object>.Default);
        }

        public static bool operator ==(KeyValueTuple<T1, T2> left, KeyValueTuple<T1, T2> right) {
            return Equals(left, right);
        }

        public static bool operator !=(KeyValueTuple<T1, T2> left, KeyValueTuple<T1, T2> right) {
            return !Equals(left, right);
        }

        public int CompareTo(object other) {
            return ((IStructuralComparable) this).CompareTo(other, Comparer<object>.Default);
        }

        public object this[int index] {
            get {
                switch(index) {
                    case 0:
                        return _key;
                    case 1:
                        return _value;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public int Length => 2;
    }
}