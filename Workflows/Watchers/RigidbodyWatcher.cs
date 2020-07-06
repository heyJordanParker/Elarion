using Elarion.Attributes;
using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Watchers {
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyWatcher : ComponentWatcher<Rigidbody> {
        [SerializeField]
        private FloatReference _totalVelocity;
        
        [SerializeField]
        private Vector3Reference _velocity;

        [SerializeField]
        private IntReference _bounces;

        public FloatReference TotalVelocity => _totalVelocity;
        public Vector3Reference Velocity => _velocity;
        public IntReference Bounces => _bounces;

        private void FixedUpdate() {
            _velocity.Value = component.velocity;
            _totalVelocity.Value = component.velocity.magnitude;
        }

        private void OnCollisionEnter2D(Collision2D other) {
            _bounces.Value++;
        }

        [InspectorButton]
        public void ResetBounces() {
            _bounces.Value = 0;
        }
    }
}