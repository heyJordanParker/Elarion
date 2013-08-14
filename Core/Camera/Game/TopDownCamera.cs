using Elarion;
using UnityEngine;

namespace Elarion {
	
	public class TopDownCamera : CameraController {

		public Ease easeType;
		public float smoothing;
		public float movementSpeed;
		public float zoomSpeed;

		public Rect cameraBounds;
		public Vector2 startingPosition;
		public float distance;
		public Vector2 distanceBounds;

		protected new Vector2 position;
		protected Vector2 targetPosition;
		protected float targetDistance;

		private Vector2 _moveDelta;

		protected override void Initialize() {
			base.Initialize();
			targetDistance = distance = distanceBounds.y;
			position = targetPosition = startingPosition;
		}

		protected virtual void LateUpdate() {
			const float epsilon = float.Epsilon;
			if(Mathf.Abs(distance - targetDistance) > epsilon) {
				distance = distance.EaseTo(targetDistance, Time.smoothDeltaTime, easeType);
			}
			if(Mathf.Abs(_moveDelta.x) > epsilon || Mathf.Abs(_moveDelta.y) > epsilon) {
				var value = Time.deltaTime * smoothing;
				targetPosition.x = Mathf.Clamp(targetPosition.x + _moveDelta.x, cameraBounds.xMin, cameraBounds.xMax);
				targetPosition.y = Mathf.Clamp(targetPosition.y + _moveDelta.y, cameraBounds.yMin, cameraBounds.yMax);
				_moveDelta = _moveDelta.EaseTo(Vector2.zero, value / movementSpeed, Ease.Linear);
			}
			if(Vector3.SqrMagnitude(position - targetPosition) > epsilon) {
				position = position.EaseTo(targetPosition, Time.deltaTime * smoothing, easeType);
			}

			Camera.transform.position = new Vector3(position.x, 0, position.y) -
							   (Camera.transform.rotation * Vector3.forward * distance);
		}

		public void Move(Vector2 delta) {
			_moveDelta.x = delta.x * movementSpeed;
			_moveDelta.y = delta.y * movementSpeed;
		}

		public void Zoom(float delta) {
			targetDistance = Mathf.Clamp(targetDistance + delta * zoomSpeed, distanceBounds.x, distanceBounds.y);
		}

		public void OnDrawGizmos() {
			Gizmos.color = Color.green;
			var anchor = new Vector3(cameraBounds.x + cameraBounds.width / 2, 5, cameraBounds.y + cameraBounds.height / 2);
			var size = new Vector3(cameraBounds.width, 10, cameraBounds.height);
			Gizmos.DrawWireCube(anchor, size);
			startingPosition.x = Mathf.Clamp(startingPosition.x, cameraBounds.xMin, cameraBounds.xMax);
			startingPosition.y = Mathf.Clamp(startingPosition.y, cameraBounds.yMin, cameraBounds.yMax);
			distance = Mathf.Clamp(distance, distanceBounds.x, distanceBounds.y);
			Camera.transform.position = new Vector3(startingPosition.x, 0, startingPosition.y) -
			                            (Camera.transform.rotation * Vector3.forward * distance);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(Camera.transform.position, 0.7f);
		}

	}
}