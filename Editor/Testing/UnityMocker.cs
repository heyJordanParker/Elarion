using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UnityEngine;

namespace Elarion.Editor.Testing {
	public class UnityMocker {

		// mock transform
		// if in unity - use game object, otherwise - use a mock game object

		private List<GameObjectMock> _gameObjects;
		private Timer _timer;

		private List<GameObjectMock> GameObjects {
			get {
				if(_gameObjects == null) 
					_gameObjects = new List<GameObjectMock>();
				return _gameObjects;
			}
		}

		private Timer Timer {
			get {
				if(_timer == null) {
					_timer = new Timer(50);
					_timer.Elapsed += (sender, args) => Update();
				}
				return _timer;
			}
		}

		public void StartUpdateLoop() {
			
		}

		public void StopUpdateLoop() {
			
		}

		private void Update() {
			foreach(var gameObject in GameObjects) {
				gameObject.FixedUpdate();
				gameObject.Update();
				gameObject.LateUpdate();
			}
		}

		public GameObjectMock CreateGameObject(string name) {
			var gameObject = new GameObjectMock(name);
			GameObjects.Add(gameObject);
			return gameObject;
		}


	}
}
