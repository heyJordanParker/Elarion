using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion {
	//when instantiated out of a pool - manually call the Start method

	//Recursive initialization? Only game objects with no parents auto initialize, otherwise the parent initializes the child objects

	// ReSharper disable InconsistentNaming
	public class ExtendedBehaviour : MonoBehaviour {

		//TODO cache all components, override AddComponent/GetComponent ( cache if not already ) to work with the cached ones and use the prewarm methods to cache the current ones

		[SerializeField, HideInInspector] private bool _initialized;

		/// <summary>
		/// Non-serialized flag indicating if this object was a copy of another object created via Instantiate.
		/// In the original object initialization will be run and the flag will be set to false. In all copies it will be deserialized 
		/// as its default - true
		/// </summary>
		protected bool _isCopy = true;
		//save the original object id; -1 if this is an original object

		protected void Awake() {
			if(!Managers.Initialized) Managers.Initialize();
			if(Managers.Session != null) Managers.Session.RegisterBehaviour(this);
			if(_initialized) return;
			Initialize();
			_initialized = true;
			_isCopy = false;
		}

		public void OnDestroy() {
			if(!_isCopy) {
				Deinitialize();
				if(Managers.Session != null) EmptyPool();
			}
			Managers.Session.RevokeBehaviour(this);
			Managers.Event.Unsubscribe(this);
		}

		/// <summary>
		/// Override this method for initializing functionality. This method is guaranteed to be called once per object.
		/// Method is not called after the object has been copied.
		/// </summary>
		protected virtual void Initialize() { }
		protected virtual void Deinitialize() { }

		//TODO override GetComponent; Cache components
		public T Component<T>() where T : Component { return gameObject.Component<T>(); }

		//TODO override SendMessage & Invoke; cache methods

		public void Subscribe(string onEvent, string executeMethod) {
			Managers.Event.Subscribe(this, onEvent, executeMethod);
		}

		// TODO Fire to all children ( send message analogue )

		public void Fire(string firedEvent) {
			Managers.Event.Fire(firedEvent);
		}

		public void Fire(string firedEvent, object parameter) {
			Managers.Event.Fire(firedEvent, parameter);
		}

		public void Broadcast(string broadcastedEvent) {
			Managers.Event.Broadcast(broadcastedEvent);
		}

		public void Broadcast(string broadcastedEvent, object parameter) {
			Managers.Event.Broadcast(broadcastedEvent, parameter);
		}

		public new Object[] FindObjectsOfType(Type type) {
			if(typeof(ExtendedBehaviour).IsAssignableFrom(type)) return Managers.Session.GetAllBehavioursOfType(type);
			return Object.FindObjectsOfType(type);
		}

		public new T[] FindObjectsOfType<T>() where T : Object {
			return Object.FindObjectsOfType(typeof(T)).Cast<T>().ToArray();
		}

		public new Coroutine StartCoroutine(IEnumerator coroutine) { return Coroutine.Create(gameObject, coroutine); }

		private void EmptyPool() {
			gameObject.EmptyPool();
		}

		public void Destroy(GameObject gameObject) {
			Managers.Pooling.Destroy(gameObject);
		}

		public void SetActive(bool value) {
			gameObject.SetActive(value);
		}

		#region ComponentsCache

		[SerializeField, HideInInspector]
		private GameObject _gameObject;
		[SerializeField, HideInInspector]
		private Rigidbody _rigidbody;
		[SerializeField, HideInInspector]
		private Transform _transform;

		public GameObject GameObject { get { return _gameObject != null ? _gameObject : (_gameObject = base.gameObject); } }
		public Rigidbody Rigidbody { get { return _rigidbody != null ? _rigidbody : (_rigidbody = base.rigidbody); } }
		public Transform Transform { get { return _transform != null ? _transform : (_transform = base.transform); } }

		public new GameObject gameObject { get { return GameObject; } }
		public new Rigidbody rigidbody { get { return Rigidbody; } }
		public new Transform transform { get { return Transform; } }

		#endregion

		#region Accessors

		public Vector3 Position {
			get { return Transform.position; }
			set { Transform.position = value; }
		}

		public Quaternion Rotation {
			get { return Transform.rotation; }
			set { Transform.rotation = value; }
		}

		public Vector3 RotationEuler {
			get { return Transform.rotation.eulerAngles; }
			set { Transform.rotation = Quaternion.Euler(value); }
		}

		public Vector3 Scale {
			get { return Transform.localScale; }
			set { Transform.localScale = value; }
		}

		public Vector3 position {
			get { return Position; }
			set { Position = value; }
		}

		public Quaternion rotation {
			get { return Rotation; }
			set { Rotation = value; }
		}

		public Vector3 rotationEuler {
			get { return RotationEuler; }
			set { RotationEuler = value; }
		}

		public Vector3 scale {
			get { return Scale; }
			set { Scale = value; }
		}

		#endregion

	}
	// ReSharper restore InconsistentNaming
}