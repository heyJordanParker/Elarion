using System;
using UnityEngine;

namespace Elarion {

	[Serializable]
	public class CameraInfo : MonoBehaviour {
		public Vector2 startingPosition;
		public Vector2 distanceBounds;
		public Vector2 xBounds;
		public Vector2 yBounds;
		public Ease easeType;
		public float moveSpeed;
		public float smoothing;
		public float zoomSpeed;
	}
}