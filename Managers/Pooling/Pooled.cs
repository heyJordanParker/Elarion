using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elarion.Managers {
	public class Pooled : ExtendedBehaviour {

		public const string OnSpawnMethod = "OnSpawn";
		public const string OnDespawnMethod = "OnDespawn";

		public Pool pool;

		public bool IsSpawned {
			get { return gameObject.activeSelf; }
		}

		void OnDestroy() {
			pool.Clear();
		}

	}
}
