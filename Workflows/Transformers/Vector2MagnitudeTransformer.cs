using Elarion.Workflows.Variables.References;
using UnityEngine;

namespace Elarion.Workflows.Transformers {
    public class Vector2MagnitudeTransformer : ExtendedBehaviour {

        [SerializeField]
        private Vector2Reference _vector;

        [SerializeField]
        private FloatReference _magnitude;

        private void OnEnable() {
            _vector.Subscribe(OnVectorChanged);
        }

        private void OnDisable() {
            _vector.Unsubscribe(OnVectorChanged);
        }

        private void OnVectorChanged(Vector2 newVector) {
            _magnitude.Value = newVector.magnitude;
        }
    }
}