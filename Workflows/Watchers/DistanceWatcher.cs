using Elarion.Attributes;
using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Watchers {
    [RequireComponent(typeof(Transform))]
    public class DistanceWatcher : ComponentWatcher<Transform> {
        
        [SerializeField]
        private FloatReference _distance;
        
        public FloatReference Distance => _distance;

        private Vector3 _previousPosition;

        private void OnEnable() {
            _previousPosition = component.position;
        }

        private void Update() {
            _distance.Value += (component.position - _previousPosition).magnitude;
            
            _previousPosition = component.position;
        }

        [InspectorButton]
        public void ResetBounces() {
            _distance.Value = 0;
            OnEnable();
        }
    }
}