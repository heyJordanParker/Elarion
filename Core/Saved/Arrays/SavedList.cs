using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Elarion.Attributes;
using Elarion.Saved.Events;
using UnityEngine;

namespace Elarion.Saved.Arrays {
    
    [SavedList]
    public class SavedList<TItem> : SavedEvent<SavedList<TItem>>, IList<TItem>, IList {
        [SerializeField, HideInInspector]
        protected List<TItem> values = new List<TItem>();

        public event Action<TItem> ItemAdded = v => { };
        public event Action<TItem> ItemRemoved = v => { };

        public override void Subscribe(Action<SavedList<TItem>> onListChanged) {
            base.Subscribe(onListChanged);
        }

        public override void Unsubscribe(Action<SavedList<TItem>> onListChanged) {
            base.Unsubscribe(onListChanged);
        }

        public IEnumerator<TItem> GetEnumerator() {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable) values).GetEnumerator();
        }

        public ReadOnlyCollection<TItem> AsReadOnly() {
            return new ReadOnlyCollection<TItem>(this);
        }

        public bool Exists(Predicate<TItem> match) {
            return values.Exists(match);
        }

        public TItem Find(Predicate<TItem> match) {
            return values.Find(match);
        }

        public List<TItem> FindAll(Predicate<TItem> match) {
            return values.FindAll(match);
        }

        public int FindIndex(Predicate<TItem> match) {
            return values.FindIndex(match);
        }

        public int FindIndex(int startIndex, Predicate<TItem> match) {
            return values.FindIndex(startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<TItem> match) {
            return values.FindIndex(startIndex, count, match);
        }

        public TItem FindLast(Predicate<TItem> match) {
            return values.FindLast(match);
        }

        public int FindLastIndex(Predicate<TItem> match) {
            return values.FindLastIndex(match);
        }

        public int FindLastIndex(int startIndex, Predicate<TItem> match) {
            return values.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<TItem> match) {
            return values.FindLastIndex(startIndex, count, match);
        }

        public int LastIndexOf(TItem item) {
            return values.LastIndexOf(item);
        }

        public int LastIndexOf(TItem item, int index) {
            return values.LastIndexOf(item, index);
        }

        public int LastIndexOf(TItem item, int index, int count) {
            return values.LastIndexOf(item, index, count);
        }

        public List<TItem> ToList() {
            return new List<TItem>(values);
        }

        public TItem[] ToArray() {
            return values.ToArray();
        }

        public void Add(TItem item) {
            values.Add(item);
            ItemAdded(item);
            Raise(this);
        }

        public void Clear() {
            var oldValues = values.ToArray();
            values.Clear();

            foreach(var value in oldValues) {
                ItemRemoved(value);
            }
            Raise(this);
        }

        public bool Contains(TItem item) {
            return values.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex) {
            values.CopyTo(array, arrayIndex);
        }

        public bool Remove(TItem item) {
            var result = values.Remove(item);
            ItemRemoved(item);
            Raise(this);
            return result;
        }

        public void CopyTo(Array array, int index) {
            ((ICollection) values).CopyTo(array, index);
        }

        public int Count {
            get { return values.Count; }
        }

        object ICollection.SyncRoot {
            get { return ((ICollection) values).SyncRoot; }
        }

        bool ICollection.IsSynchronized {
            get { return false; }
        }

        bool ICollection<TItem>.IsReadOnly {
            get { return false; }
        }

        public int IndexOf(TItem item) {
            return values.IndexOf(item);
        }

        public void Insert(int index, TItem item) {
            values.Insert(index, item);
            ItemAdded(item);
            Raise(this);
        }

        public void RemoveAt(int index) {
            var value = values[index];
            values.RemoveAt(index);
            ItemRemoved(value);
            Raise(this);
        }

        public TItem this[int index] {
            get { return values[index]; }
            set {
                var oldValue = values[index];

                if(oldValue.Equals(value)) {
                    return;
                }
                
                values[index] = value;

                ItemRemoved(oldValue);
                ItemAdded(value);
                Raise(this);
            }
        }

        int IList.Add(object value) {
            var result = ((IList) values).Add(value);
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
            get { return this[index]; }
            set { this[index] = (TItem) value; }
        }

        bool IList.IsReadOnly {
            get { return ((IList) values).IsReadOnly; }
        }

        bool IList.IsFixedSize {
            get { return ((IList) values).IsFixedSize; }
        }
    }
}