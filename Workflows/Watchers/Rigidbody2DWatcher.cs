using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Watchers {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Rigidbody2DWatcher : ComponentWatcher<Rigidbody2D> {
        [SerializeField]
        private FloatReference _totalVelocity;
        
        [SerializeField]
        private Vector2Reference _velocity;

        public FloatReference TotalVelocity => _totalVelocity;
        public Vector2Reference Velocity => _velocity;

        private void FixedUpdate() {
            _velocity.Value = component.velocity;
            _totalVelocity.Value = component.velocity.magnitude;
        }
    }
}