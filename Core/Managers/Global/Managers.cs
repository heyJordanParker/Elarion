using System;
using System.Collections.Generic;
using Elarion.CoreManagers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Elarion {

	public static class Managers {

		#region Managers Quick Access
		public static AudioManager Audio { get { return _audio; } set { if(_audio == null) _audio = value; } }
		public static CoroutineSchedule Coroutine { get { return _coroutine; } set { if(_coroutine == null) _coroutine = value; } }
		public static ResourcesManager Resources { get { return _resources; } set { if(_resources == null) _resources = value; } }
		public static QualityManager Quality { get { return _quality; } set { if(_quality == null) _quality = value; } }
		public static PoolingManager Pooling { get { return _pooling; } set { if(_pooling == null) _pooling = value; } }
		public static SessionManager Session { get { return _session; } set { if(_session == null) _session = value; } }
		public static InputManager Input { get { return _input; } set { if(_input == null) _input = value; } }
		internal static LoadingManager Loading { get { return _loading; } set { if(_loading == null) _loading = value; } }
		internal static EventManager Event { get { return _event; } set { if(_event == null) _event = value; } }

		private static PoolingManager _pooling;
		private static AudioManager _audio;
		private static CoroutineSchedule _coroutine;
		private static ResourcesManager _resources;
		private static QualityManager _quality;
		private static SessionManager _session;
		private static InputManager _input;
		private static LoadingManager _loading;
		private static EventManager _event;

		private static readonly Dictionary<Type, Component> DefaultManagers = new Dictionary<Type, Component>();
		private static bool _initialized;
		#endregion

		public static void Initialize() {
			if(_initialized) return;
			_initialized = true;
			Pooling = InitializeDefaultManager<PoolingManager>();
			Coroutine = InitializeDefaultManager<CoroutineSchedule>();
			Audio = InitializeDefaultManager<AudioManager>();
			Resources = InitializeDefaultManager<ResourcesManager>();
			Quality = InitializeDefaultManager<QualityManager>();
			Session = InitializeDefaultManager<SessionManager>();
			Input = InitializeDefaultManager<InputManager>();
			Event = InitializeDefaultManager<EventManager>();
			Loading = InitializeDefaultManager<LoadingManager>();
		}

		private static T InitializeDefaultManager<T>() where T : Component {
			Component cachedComponent;
			if(DefaultManagers.TryGetValue(typeof(T), out cachedComponent)) {
				if(cachedComponent == null)
					DefaultManagers.Remove(typeof(T));
				else
					return cachedComponent as T;
			}
			T manager;
			var instance = GameObject.FindObjectOfType(typeof(T)) as T;
			if(instance == null) {
				var go = new GameObject(typeof(T).Name);
				manager = go.AddComponent<T>();
			} else {
				manager = instance;
			}
			Object.DontDestroyOnLoad(manager);
			DefaultManagers.Add(typeof(T), manager);
			return manager;
		}

		public static bool Initialized { get { return _initialized; } }

	}

}