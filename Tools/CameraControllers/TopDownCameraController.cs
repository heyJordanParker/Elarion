using Elarion.Common;
using Elarion.Common.Attributes;
using Elarion.Common.Extensions;
using UnityEngine;

namespace Elarion.Tools.CameraControllers {
    [RequireComponent(typeof(Camera))]
    public class TopDownCameraController : ExtendedBehaviour {
        
        public Ease easeType;
        public float smoothing;
        public float movementSpeed;
        public float zoomSpeed;

        public Rect cameraBounds;
        public Vector2 startingPosition;
        public float distance;
        public Vector2 distanceBounds;

        protected Vector2 position;
        protected Vector2 targetPosition;
        protected float targetDistance;

        private Vector2 _moveDelta;

        [SerializeField, GetComponent]
        private Camera _camera;

        [SerializeField, GetComponent]
        private Transform _transform;

        protected void Awake() {
            targetDistance = distance = distanceBounds.y;
            position = targetPosition = startingPosition;
        }

        protected virtual void LateUpdate() {
            if(Mathf.Abs(distance - targetDistance) > float.Epsilon) {
                distance = distance.EaseTo(targetDistance, Time.smoothDeltaTime, easeType);
            }

            if(Mathf.Abs(_moveDelta.x) > float.Epsilon || Mathf.Abs(_moveDelta.y) > float.Epsilon) {
                var value = Time.deltaTime * smoothing;
                targetPosition.x = Mathf.Clamp(targetPosition.x + _moveDelta.x, cameraBounds.xMin, cameraBounds.xMax);
                targetPosition.y = Mathf.Clamp(targetPosition.y + _moveDelta.y, cameraBounds.yMin, cameraBounds.yMax);
                _moveDelta = _moveDelta.EaseTo(Vector2.zero, value / movementSpeed, Ease.Linear);
            }

            if(Vector3.SqrMagnitude(position - targetPosition) > float.Epsilon) {
                position = position.EaseTo(targetPosition, Time.deltaTime * smoothing, easeType);
            }

            _transform.position = new Vector3(position.x, 0, position.y) -
                                         (_transform.rotation * Vector3.forward * distance);
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
            var anchor = new Vector3(cameraBounds.x + cameraBounds.width / 2, 5,
                cameraBounds.y + cameraBounds.height / 2);
            var size = new Vector3(cameraBounds.width, 10, cameraBounds.height);
            Gizmos.DrawWireCube(anchor, size);
            startingPosition.x = Mathf.Clamp(startingPosition.x, cameraBounds.xMin, cameraBounds.xMax);
            startingPosition.y = Mathf.Clamp(startingPosition.y, cameraBounds.yMin, cameraBounds.yMax);
            distance = Mathf.Clamp(distance, distanceBounds.x, distanceBounds.y);
            _transform.position = new Vector3(startingPosition.x, 0, startingPosition.y) -
                                         (_transform.rotation * Vector3.forward * distance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_transform.position, 0.7f);
        }
    }
}