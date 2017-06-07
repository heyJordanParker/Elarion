using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Elarion.Editor.Testing {
	public class GameObjectMock {
		private enum SpecialFunctionName { FixedUpdate, Update, LateUpdate };

		private Dictionary<Component, Dictionary<SpecialFunctionName, Action>> _cachedFunctions;

		public string Name { get; private set; }

		public List<Component> Components { get; private set; }

		public Transform transform { get; private set; }

		private Dictionary<Component, Dictionary<SpecialFunctionName, Action>> CachedFunctions {
			get {
				if(_cachedFunctions == null) {
					_cachedFunctions = new Dictionary<Component, Dictionary<SpecialFunctionName, Action>>();
				}
				return _cachedFunctions;
			}
		}

		public GameObjectMock(string name) {
			Name = name;
			Components = new List<Component>();
			transform = AddComponent<Transform>();
		}

		public T AddComponent<T>() where T : Component {
			T component = Activator.CreateInstance(typeof(Transform),
				BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, new object[] { }, null) as T;
			CacheFunctions(component);
			Components.Add(component);
			return component;
		}

		public T GetComponent<T>() where T : Component {
			return Components.FirstOrDefault(component => component.GetType() == typeof(T)) as T;
		}

		public void RemoveComponent(Component component) {
			CachedFunctions.Remove(component);
			Components.Remove(component);
		}

		public void FixedUpdate() {}
		public void Update() {}
		public void LateUpdate() {}

		private void CacheFunctions(Component c) {
			var type = c.GetType();
		}
	}
}
