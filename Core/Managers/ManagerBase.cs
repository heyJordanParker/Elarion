using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Elarion {
	public class ManagerBase : AsyncLoad {

		public Resource[] resources;

		protected override void Initialize() {
			base.Initialize();
			foreach(var resource in resources)
				Managers.Resources.Add(resource);
		}

		protected override void Deinitialize() {
			base.Deinitialize();
			foreach(var resource in resources)
				Managers.Resources.Remove(resource);
		}

	}
}
