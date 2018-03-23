using System;
#if !CompactFramework
using System.Runtime.Serialization;
#endif
using System.Collections.Generic;
using System.Diagnostics;

namespace Elarion {

	/// <summary>
	/// A dictionary in which the values are weak references. Written by DLP for SWIG.
	/// </summary>
	/// <remarks>
	/// Null values are not allowed in this dictionary.
	/// 
	/// When a value is garbage-collected, the dictionary acts as though the key is
	/// not present.
	/// 
	/// This class "cleans up" periodically by removing entries with garbage-collected
	/// values. Cleanups only occur occasionally, and only when the dictionary is accessed;
	/// Accessing it (for read or write) more often results in more frequent cleanups.
	///
	/// Watch out! The following interface members are not implemented:
	/// IDictionary.Values, ICollection.Contains, ICollection.CopyTo, ICollection.Remove.
	/// Also, the dictionary is NOT MULTITHREAD-SAFE.
	/// </remarks>
	/// <source>
	/// https://gist.github.com/qwertie/3867055
	/// </source>
	public class WeakValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>
		where TValue : class {
		Dictionary<TKey, WeakReference<TValue>> _dict = new Dictionary<TKey, WeakReference<TValue>>();
		int _version, _cleanVersion;
#if !CompactFramework
		int _cleanGeneration;
#endif
		const int MinRehashInterval = 500;

		public WeakValueDictionary() {
		}

		#region IDictionary<TKey,TValue> Members

		public ICollection<TKey> Keys {
			get { return _dict.Keys; }
		}
		public ICollection<TValue> Values {	// TODO. Maybe. Eventually.
			get { throw new NotImplementedException(); }
		}

		public bool ContainsKey(TKey key) {
			AutoCleanup(1);

			WeakReference<TValue> value;
			if(!_dict.TryGetValue(key, out value))
				return false;
			return value.IsAlive;
		}
		public void Add(TKey key, TValue value) {
			AutoCleanup(2);

			WeakReference<TValue> wr;
			if(_dict.TryGetValue(key, out wr)) {
				if(wr.IsAlive)
					throw new ArgumentException("An element with the same key already exists in this WeakValueDictionary");
				else
					wr.Target = value;
			} else
				_dict.Add(key, new WeakReference<TValue>(value));
		}
		public bool Remove(TKey key) {
			AutoCleanup(1);

			WeakReference<TValue> wr;
			if(!_dict.TryGetValue(key, out wr))
				return false;
			_dict.Remove(key);
			return wr.IsAlive;
		}
		public bool TryGetValue(TKey key, out TValue value) {
			AutoCleanup(1);

			WeakReference<TValue> wr;
			if(_dict.TryGetValue(key, out wr))
				value = wr.Target;
			else
				value = null;
			return value != null;
		}

		public TValue this[TKey key] {
			get {
				return _dict[key].Target;
			}
			set {
				_dict[key] = new WeakReference<TValue>(value);
			}
		}

		void AutoCleanup(int incVersion) {
			_version += incVersion;

			// Cleanup the table every so often--less often for larger tables.
			long delta = _version - _cleanVersion;
			if(delta > MinRehashInterval + _dict.Count) {
#if CompactFramework
				Cleanup();
				_cleanVersion = _version;
#else
				// A cleanup will be fruitless unless a GC has happened in the meantime.
				// WeakReferences can become zero only during the GC.
				int curGeneration = GC.CollectionCount(0);
				if(_cleanGeneration != curGeneration) {
					_cleanGeneration = curGeneration;
					Cleanup();
					_cleanVersion = _version;
				} else
					_cleanVersion += MinRehashInterval; // Wait a little while longer
#endif
			}
		}
		void Cleanup() {
			// Remove all pairs whose value is nullified.
			// Due to the fact that you can't change a Dictionary while enumerating 
			// it, we need an intermediate collection (the list of things to delete):
			List<TKey> deadKeys = new List<TKey>();

			foreach(KeyValuePair<TKey, WeakReference<TValue>> kvp in _dict)
				if(!kvp.Value.IsAlive)
					deadKeys.Add(kvp.Key);

			foreach(TKey key in deadKeys) {
				bool success = _dict.Remove(key);
				Debug.Assert(success);
			}
		}
		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Add(KeyValuePair<TKey, TValue> item) {
			Add(item.Key, item.Value);
		}
		public void Clear() {
			_dict.Clear();
			_version = _cleanVersion = 0;
#if !CompactFramework
			_cleanGeneration = 0;
#endif
		}
		public bool Contains(KeyValuePair<TKey, TValue> item) {
			throw new Exception("The method or operation is not implemented.");
		}
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			throw new Exception("The method or operation is not implemented.");
		}
		public int Count {
			// THIS VALUE MAY BE WRONG (i.e. it may be higher than the number of 
			// items you get from the iterator).
			get { return _dict.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(KeyValuePair<TKey, TValue> item) {
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			int nullCount = 0;

			foreach(KeyValuePair<TKey, WeakReference<TValue>> kvp in _dict) {
				TValue target = kvp.Value.Target;
				if(target == null)
					nullCount++;
				else
					yield return new KeyValuePair<TKey, TValue>(kvp.Key, target);
			}

			if(nullCount > _dict.Count / 4)
				Cleanup();
		}

		#endregion
	}

	public class WeakReference<T> : WeakReference {
		public WeakReference(T target) : base(target) { }
		public WeakReference(T target, bool trackResurrection) : base(target, trackResurrection) { }
#if !CompactFramework
		protected WeakReference(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public new T Target {
			get { return (T)base.Target; }
			set { base.Target = value; }
		}
	}
}
