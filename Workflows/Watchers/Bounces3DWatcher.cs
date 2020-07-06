using Elarion.Attributes;
using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Watchers {
    [RequireComponent(typeof(Collider))]
    public class Bounces3DWatcher : ComponentWatcher<Collider> {
        
        [SerializeField]
        private IntReference _bounces;
        
        public IntReference Bounces => _bounces;
        
        private void OnCollisionEnter(Collision other) {
            _bounces.Value++;
        }

        [InspectorButton]
        public void ResetBounces() {
            _bounces.Value = 0;
        }
    }
}