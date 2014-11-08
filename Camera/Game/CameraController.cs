using UnityEngine;

namespace Elarion {
	[RequireComponent(typeof(Camera))]
	public class CameraController : ExtendedBehaviour {

		private Camera _camera;

		protected Camera Camera { get { return _camera == null ? (_camera = GetComponent<Camera>()) : _camera; } }

	}
}