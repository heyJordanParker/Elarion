using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Watchers {
    public class TransformWatcher : ComponentWatcher<Transform> {
        [SerializeField]
        private Vector3Reference _position;
        
        [SerializeField]
        private Vector3Reference _rotation;
        
        [SerializeField]
        private Vector3Reference _scale;

        public Vector3Reference Position => _position;
        public Vector3Reference Rotation => _rotation;
        public Vector3Reference Scale => _scale;

        protected virtual void Update() {
            _position.Value = component.position;
            _rotation.Value = component.rotation.eulerAngles;
            _scale.Value = component.lossyScale;
        }
    }
}