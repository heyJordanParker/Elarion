using Elarion.Attributes;
using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Watchers {
    [RequireComponent(typeof(Collider2D))]
    public class Bounces2DWatcher : ComponentWatcher<Collider2D> {
        
        [SerializeField]
        private IntReference _bounces;
        
        public IntReference Bounces => _bounces;
        
        private void OnCollisionEnter2D(Collision2D other) {
            _bounces.Value++;
        }

        [InspectorButton]
        public void ResetBounces() {
            _bounces.Value = 0;
        }
    }
}