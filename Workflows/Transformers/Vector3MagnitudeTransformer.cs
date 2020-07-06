using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Transformers {
    public class Vector3MagnitudeTransformer : ExtendedBehaviour {

        [SerializeField]
        private Vector3Reference _vector;

        [SerializeField]
        private FloatReference _magnitude;

        private void OnEnable() {
            _vector.Subscribe(OnVectorChanged);
        }

        private void OnDisable() {
            _vector.Unsubscribe(OnVectorChanged);
        }

        private void OnVectorChanged(Vector3 newVector) {
            _magnitude.Value = newVector.magnitude;
        }
    }
}