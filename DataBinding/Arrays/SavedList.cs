using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Elarion.Common.Attributes;
using Elarion.DataBinding.Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Elarion.DataBinding.Arrays {
    
    [SavedList]
    public class SavedList<TItem> : SavedEvent<SavedList<TItem>>, IList<TItem>, IList {
        [FormerlySerializedAs("values")]
        [SerializeField]
        private List<TItem> _initialValues = new List<TItem>();

        [SerializeField]
        private List<TItem> _runtimeValues;

        public event Action<TItem> ItemAdded = v => { };
        public event Action<TItem> ItemRemoved = v => { };

        protected List<TItem> Values => _runtimeValues;

        protected override void OnEnable() {
            base.OnEnable();
            
            _runtimeValues = new List<TItem>();

            Reset();
        }

        protected override void OnDisable() {
            base.OnDisable();
            
            _runtimeValues = null;
        }

        public void Reset() {
            Values.Clear();
            Values.AddRange(_initialValues);
        }
        
        public override void Subscribe(Action<SavedList<TItem>> onListChanged) {
            base.Subscribe(onListChanged);
        }

        public override void Unsubscribe(Action<SavedList<TItem>> onListChanged) {
            base.Unsubscribe(onListChanged);
        }

        public IEnumerator<TItem> GetEnumerator() {
            return (Values as IEnumerable<TItem>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) Values).GetEnumerator();
        }

        public ReadOnlyCollection<TItem> AsReadOnly() {
            return new ReadOnlyCollection<TItem>(this);
        }

        public bool Exists(Predicate<TItem> match) {
            return Values.Exists(match);
        }

        public TItem Find(Predicate<TItem> match) {
            return Values.Find(match);
        }

        public List<TItem> FindAll(Predicate<TItem> match) {
            return Values.FindAll(match);
        }

        public int FindIndex(Predicate<TItem> match) {
            return Values.FindIndex(match);
        }

        public int FindIndex(int startIndex, Predicate<TItem> match) {
            return Values.FindIndex(startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<TItem> match) {
            return Values.FindIndex(startIndex, count, match);
        }

        public TItem FindLast(Predicate<TItem> match) {
            return Values.FindLast(match);
        }

        public int FindLastIndex(Predicate<TItem> match) {
            return Values.FindLastIndex(match);
        }

        public int FindLastIndex(int startIndex, Predicate<TItem> match) {
            return Values.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<TItem> match) {
            return Values.FindLastIndex(startIndex, count, match);
        }

        public int LastIndexOf(TItem item) {
            return Values.LastIndexOf(item);
        }

        public int LastIndexOf(TItem item, int index) {
            return Values.LastIndexOf(item, index);
        }

        public int LastIndexOf(TItem item, int index, int count) {
            return Values.LastIndexOf(item, index, count);
        }

        public List<TItem> ToList() {
            return new List<TItem>(Values);
        }

        public TItem[] ToArray() {
            return Values.ToArray();
        }

        public void Add(TItem item) {
            Values.Add(item);
            ItemAdded(item);
            Raise(this);
        }

        public void Clear() {
            var oldValues = Values.ToArray();
            Values.Clear();

            foreach(var value in oldValues) {
                ItemRemoved(value);
            }
            Raise(this);
        }

        public bool Contains(TItem item) {
            return Values.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex) {
            Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(TItem item) {
            var result = Values.Remove(item);
            ItemRemoved(item);
            Raise(this);
            return result;
        }

        public void CopyTo(Array array, int index) {
            ((ICollection) Values).CopyTo(array, index);
        }

        public int Count => Values.Count;

        object ICollection.SyncRoot => ((ICollection) Values).SyncRoot;

        bool ICollection.IsSynchronized => false;

        bool ICollection<TItem>.IsReadOnly => false;

        public int IndexOf(TItem item) {
            return Values.IndexOf(item);
        }

        public void Insert(int index, TItem item) {
            Values.Insert(index, item);
            ItemAdded(item);
            Raise(this);
        }

        public void RemoveAt(int index) {
            var value = Values[index];
            Values.RemoveAt(index);
            ItemRemoved(value);
            Raise(this);
        }

        public TItem this[int index] {
            get => Values[index];
            set {
                var oldValue = Values[index];

                if(oldValue.Equals(value)) {
                    return;
                }
                
                Values[index] = value;

                ItemRemoved(oldValue);
                ItemAdded(value);
                Raise(this);
            }
        }

        int IList.Add(object value) {
            var result = ((IList) Values).Add(value);
            ItemAdded((TItem) value);
            Raise(this);
            return result;
        }

        bool IList.Contains(object value) {
            return Contains((TItem) value);
        }

        void IList.Clear() {
            Clear();
        }

        int IList.IndexOf(object value) {
            return IndexOf((TItem) value);
        }

        void IList.Insert(int index, object value) {
            Insert(index, (TItem) value);
        }

        void IList.Remove(object value) {
            Remove((TItem) value);
        }

        void IList.RemoveAt(int index) {
            RemoveAt(index);
        }

        object IList.this[int index] {
            get => this[index];
            set => this[index] = (TItem) value;
        }

        bool IList.IsReadOnly => ((IList) Values).IsReadOnly;

        bool IList.IsFixedSize => ((IList) Values).IsFixedSize;
        
        
        [InspectorButton("Save RuntimeValues as InitialValues")]
        protected void Save() {
            if(!Application.isPlaying) {
                Debug.LogWarning("Can't save runtime values outside of play mode.");
                return;
            }

            _initialValues.Clear();
            _initialValues.AddRange(_runtimeValues);
        }
    }
}