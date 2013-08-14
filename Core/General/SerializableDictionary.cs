using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elarion {
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
	
		//TODO Create an Serializable Dictionary Inspector
		//TODO Behave like a standard dictionary when out of the editor ( and emptying the key & value arrays after initialization )
		
		[SerializeField] private List<TKey> _keys;
		[SerializeField] private List<TValue> _values;

		private Dictionary<TKey, TValue> _data;

		public TValue this[TKey key] {
			get { return Get(key); }
			set { Add(key, value); }
		}

		public SerializableDictionary(IEqualityComparer<TKey> equalityComparer = null) {
			if(_keys != null) {
				if(_values == null) _values = new List<TValue>(_keys.Capacity);
				Initialize(equalityComparer);
			} else {
				_keys = new List<TKey>();
				_values = new List<TValue>();
				Count = 0;
			}
		}

		private void Initialize(IEqualityComparer<TKey> equalityComparer) {
			_data = new Dictionary<TKey, TValue>(equalityComparer);
			if(_keys.Count <= 0) return;
			Count = _keys.Count;
			for(int i = 0; i < Count; ++i) {
				Data.Add(_keys[i], _values[i]);
			}
		}

		public TValue Get(TKey key) {
			TValue val;
			Data.TryGetValue(key, out val);
			return val;
		}

		public bool ContainsKey(TKey key) { return false; }

		public void Add(TKey key, TValue value) {
			for(int i = 0, count = _keys.Count; i < count; ++i) {
				if(!_keys[i].Equals(key))
					continue;
				Data[key] = value;
				_values[i] = value;
				return;
			}
			Data.Add(key, value);
			_keys.Add(key);
			_values.Add(value);
			++Count;
		}

		public bool Remove(TKey key) { return false; }

		public ICollection<TValue> Values {
			get { return Data.Values; }
		}

		public ICollection<TKey> Keys {
			get { return Data.Keys; }
		}

		private Dictionary<TKey, TValue> Data { get { if(_data == null) Initialize(null); return _data; } }

		public bool TryGetValue(TKey key, out TValue value) {
			return Data.TryGetValue(key, out value);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return Data.GetEnumerator(); }

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

		public void Add(KeyValuePair<TKey, TValue> item) { Add(item.Key, item.Value); }

		public void Clear() {
			_keys = new List<TKey>();
			_values = new List<TValue>();
			_data = new Dictionary<TKey, TValue>();
			Count = 0;
		}
		public bool Contains(KeyValuePair<TKey, TValue> item) { return _data.ContainsKey(item.Key); }
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			var newCount = array.Length + Data.Count;
			var newArray = new KeyValuePair<TKey, TValue>[newCount];
			for(int i = 0, j = 0; i < newCount; ++i, ++j) {
				if(i == arrayIndex) {
					foreach(var kvp in Data) {
						newArray[i] = kvp;
						++i;
					}
				} else {
					newArray[i] = array[j];
				}
			}
		}
		public bool Remove(KeyValuePair<TKey, TValue> item) {
			return _data.Remove(item.Key) && _keys.Remove(item.Key) && _values.Remove(item.Value);
		}
		public int Count { get; private set; }
		public bool IsReadOnly { get; private set; }
	}

}